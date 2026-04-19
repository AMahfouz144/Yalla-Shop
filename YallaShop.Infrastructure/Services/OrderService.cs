using Microsoft.EntityFrameworkCore;
using YallaShop.Application;
using YallaShop.Application.DTOs;
using YallaShop.Application.DTOs.ShippingAddress;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly AppDbContext _context;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, AppDbContext context)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _context = context;
        }

        // ─── Queries (Using PROJECTION - No Include) ────────────────────────────────

        public async Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(string userId)
        {
            try
            {
                var orderDtos = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(OrderSelector)
                    .ToListAsync();

                return new ResponseModel<IEnumerable<OrderResponseDto>> { IsSuccess = true, Data = orderDtos };
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<OrderResponseDto>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerId)
        {
            try
            {
                var orderDtos = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.UserId == customerId)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(OrderSelector)
                    .ToListAsync();

                return new ResponseModel<IEnumerable<OrderResponseDto>> { IsSuccess = true, Data = orderDtos };
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<OrderResponseDto>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetOrdersBySellerAsync(string sellerId)
        {
            try
            {
                var orderDtos = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.Items.Any(i => i.Product.Seller.UserId == sellerId))
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(OrderSelector)
                    .ToListAsync();

                return new ResponseModel<IEnumerable<OrderResponseDto>> { IsSuccess = true, Data = orderDtos };
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<OrderResponseDto>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel<OrderResponseDto>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var orderDto = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.Id == orderId)
                    .Select(OrderSelector)
                    .FirstOrDefaultAsync();

                if (orderDto == null)
                    return new ResponseModel<OrderResponseDto> { IsSuccess = false, Message = "Order not found." };

                return new ResponseModel<OrderResponseDto> { IsSuccess = true, Data = orderDto };
            }
            catch (Exception ex)
            {
                return new ResponseModel<OrderResponseDto> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel<OrderStatus>> GetOrderStatusAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .AsNoTracking()
                    .Select(o => new { o.Id, o.Status })
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null) return new ResponseModel<OrderStatus> { IsSuccess = false, Message = "Order not found." };
                return new ResponseModel<OrderStatus> { IsSuccess = true, Data = order.Status };
            }
            catch (Exception ex) { return new ResponseModel<OrderStatus> { IsSuccess = false, Message = ex.Message }; }
        }

        // ─── Commands ────────────────────────────────────────────────────────────────

        public async Task<ResponseModel<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                // We fetch by ID for update - IOrderRepository usually handles this efficiently
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null) return new ResponseModel<bool> { IsSuccess = false, Data = false };

                order.Status = status;
                _orderRepository.Update(order);
                await _context.SaveChangesAsync();

                return new ResponseModel<bool> { IsSuccess = true, Data = true };
            }
            catch (Exception ex) { return new ResponseModel<bool> { IsSuccess = false, Message = ex.Message, Data = false }; }
        }

        public async Task<ResponseModel<bool>> CancelOrderAsync(int orderId)
        {
            try
            {
                // Logic requires items to restore stock
                var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null || order.Status != OrderStatus.Pending) return new ResponseModel<bool> { IsSuccess = false, Data = false };

                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _productRepository.Update(product);
                    }
                }

                order.Status = OrderStatus.Cancelled;
                _orderRepository.Update(order);
                await _context.SaveChangesAsync();

                return new ResponseModel<bool> { IsSuccess = true, Data = true };
            }
            catch (Exception ex) { return new ResponseModel<bool> { IsSuccess = false, Message = ex.Message, Data = false }; }
        }

        // ─── Shared Selector (Projection) ───────────────────────────────────────────

        private static readonly System.Linq.Expressions.Expression<Func<Order, OrderResponseDto>> OrderSelector = o => new OrderResponseDto
        {
            Id = o.Id,
            Status = o.Status,
            TotalPrice = o.TotalPrice,
            CreatedAt = o.CreatedAt,
            ShippingAddress = o.ShippingAddress == null ? null : new ShippingAddressDto
            {
                Id = o.ShippingAddress.Id,
                Label = o.ShippingAddress.Label,
                Street = o.ShippingAddress.Street,
                City = o.ShippingAddress.City,
                State = o.ShippingAddress.State,
                Country = o.ShippingAddress.Country,
                ZipCode = o.ShippingAddress.ZipCode,
                IsDefault = o.ShippingAddress.IsDefault
            },
            PaymentMethod = o.Payment != null ? o.Payment.Method.ToString() : null,
            PaymentStatus = o.Payment != null ? o.Payment.Status.ToString() : null,
            Items = o.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product != null ? i.Product.Name : "Unknown Product",
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }
}
