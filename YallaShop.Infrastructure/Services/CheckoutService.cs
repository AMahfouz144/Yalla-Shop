using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using YallaShop.Application;
using YallaShop.Application.DTOs.Checkout;
using YallaShop.Application.DTOs.ShippingAddress;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IGenericRepository<PromoCode> _promoRepo;
        private readonly IStripeService _stripeService;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _context;

        public CheckoutService(
            ICartRepository cartRepo,
            IGenericRepository<PromoCode> promoRepo,
            IStripeService stripeService,
            IEmailSender emailSender,
            AppDbContext context)
        {
            _cartRepo = cartRepo;
            _promoRepo = promoRepo;
            _stripeService = stripeService;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<ResponseModel<OrderDto>> CheckoutAsync(CheckoutDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod) ||
                    paymentMethod is PaymentMethod.None)
                {
                    await transaction.RollbackAsync();
                    return new ResponseModel<OrderDto> { IsSuccess = false, Message = "Invalid payment method" };
                }

                var cart = await _cartRepo.GetCartWithItemsAsync(dto.UserId, dto.GuestSessionId);
                if (cart == null || cart.CartItems.Count == 0)
                {
                    await transaction.RollbackAsync();
                    return new ResponseModel<OrderDto> { IsSuccess = false, Message = "Cart is empty" };
                }

                foreach (var item in cart.CartItems)
                {
                    if (item.Product == null || item.Product.IsDeleted)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseModel<OrderDto> { IsSuccess = false, Message = "A cart item product is invalid" };
                    }

                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseModel<OrderDto>
                        {
                            IsSuccess = false,
                            Message = $"Insufficient stock for {item.Product.Name}"
                        };
                    }
                }

                var address = await _context.ShippingAddresses
                    .FirstOrDefaultAsync(a =>
                        a.Id == dto.ShippingAddressId &&
                        !a.IsDeleted &&
                        (a.UserId == dto.UserId || dto.GuestSessionId != null));

                if (address == null)
                {
                    await transaction.RollbackAsync();
                    return new ResponseModel<OrderDto> { IsSuccess = false, Message = "Invalid shipping address" };
                }

                var subTotal = cart.CartItems.Sum(i => i.Product!.Price * i.Quantity);
                var shippingCost = subTotal >= 500 ? 0 : 50;
                decimal discount = 0;
                PromoCode? promo = null;

                if (!string.IsNullOrWhiteSpace(dto.PromoCode))
                {
                    var code = dto.PromoCode.Trim().ToUpper();
                    promo = await _promoRepo.GetAllAsync().FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);
                    if (promo == null || !promo.IsActive || DateTime.UtcNow < promo.StartDate ||
                        DateTime.UtcNow > promo.EndDate)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseModel<OrderDto> { IsSuccess = false, Message = "Invalid promo code" };
                    }

                    if (promo.MinOrderAmount.HasValue && subTotal < promo.MinOrderAmount.Value)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseModel<OrderDto>
                        {
                            IsSuccess = false,
                            Message = $"Minimum order is {promo.MinOrderAmount}"
                        };
                    }

                    if (promo.MaxUsageCount.HasValue && promo.UsedCount >= promo.MaxUsageCount.Value)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseModel<OrderDto> { IsSuccess = false, Message = "Promo usage limit reached" };
                    }

                    discount = promo.DiscountType == DiscountType.Percentage
                        ? subTotal * (promo.DiscountValue / 100m)
                        : promo.DiscountValue;
                    promo.UsedCount += 1;
                    _promoRepo.Update(promo);
                }

                var totalAmount = Math.Max(0, subTotal + shippingCost - discount);
                var order = new Order
                {
                    UserId = dto.UserId ?? "GUEST",
                    Status = OrderStatus.Pending,
                    TotalPrice = totalAmount,
                    ShippingAddressId = dto.ShippingAddressId,
                    Items = new List<OrderItem>()
                };

                await _context.Orders.AddAsync(order);

                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Product!.Price
                    };
                    order.Items.Add(orderItem);

                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                    _context.Products.Update(cartItem.Product);
                }

                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();

                var payment = new Payment
                {
                    OrderId = order.Id,
                    Method = paymentMethod,
                    Status = PaymentStatus.Pending,
                    Amount = totalAmount,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Payments.AddAsync(payment);

                if (dto.PaymentMethod.Equals("Stripe", StringComparison.OrdinalIgnoreCase))
                {
                    var (clientSecret, transactionId) =
                        await _stripeService.CreatePaymentIntentAsync(totalAmount, order.Id);
                    payment.StripeClientSecret = clientSecret;
                    payment.TransactionId = transactionId;
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(dto.GuestEmail))
                {
                    await _emailSender.SendEmailAsync(dto.GuestEmail, "YallaShop order confirmation",
                        $"Your order #{order.Id} was placed successfully.");
                }

                await transaction.CommitAsync();

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    OrderNumber = $"YS-{order.Id:D6}",
                    SubTotal = subTotal,
                    ShippingCost = shippingCost,
                    Discount = discount,
                    TotalAmount = totalAmount,
                    Status = order.Status.ToString(),
                    PaymentMethod = dto.PaymentMethod,
                    PaymentStatus = PaymentStatus.Pending.ToString(),
                    StripeClientSecret = payment.StripeClientSecret,
                    ShippingAddress = new ShippingAddressDto
                    {
                        Id = address.Id,
                        Label = address.Label,
                        Street = address.Street,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        ZipCode = address.ZipCode,
                        IsDefault = address.IsDefault
                    }
                };

                return new ResponseModel<OrderDto>
                {
                    IsSuccess = true,
                    Message = "Checkout completed",
                    Data = orderDto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseModel<OrderDto>
                {
                    IsSuccess = false,
                    Message = "Checkout failed: " + ex.Message
                };
            }
        }
    }
}
