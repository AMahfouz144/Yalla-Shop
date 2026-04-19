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

        public async Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.Payment)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var orderDtos = orders.Select(o => MapOrderResponse(o));

                return new ResponseModel<IEnumerable<OrderResponseDto>>
                {
                    IsSuccess = true,
                    Data = orderDtos
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<OrderResponseDto>>
                {
                    IsSuccess = false,
                    Message = "Error fetching orders: " + ex.Message
                };
            }
        }

        public async Task<ResponseModel<OrderResponseDto>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ResponseModel<OrderResponseDto> { IsSuccess = false, Message = "Order not found." };
                }

                return new ResponseModel<OrderResponseDto>
                {
                    IsSuccess = true,
                    Data = MapOrderResponse(order)
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<OrderResponseDto>
                {
                    IsSuccess = false,
                    Message = "Error fetching order details: " + ex.Message
                };
            }
        }

        private static OrderResponseDto MapOrderResponse(Order o)
        {
            return new OrderResponseDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                ShippingAddress = o.ShippingAddress == null
                    ? null
                    : new ShippingAddressDto
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
                PaymentMethod = o.Payment?.Method.ToString(),
                PaymentStatus = o.Payment?.Status.ToString(),
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Unknown Product",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }

        public async Task<ResponseModel<OrderStatus>> GetOrderStatusAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ResponseModel<OrderStatus> { IsSuccess = false, Message = "Order not found." };
                }

                return new ResponseModel<OrderStatus>
                {
                    IsSuccess = true,
                    Data = order.Status
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<OrderStatus>
                {
                    IsSuccess = false,
                    Message = "Error fetching order status: " + ex.Message
                };
            }
        }

        public async Task<ResponseModel<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ResponseModel<bool> { IsSuccess = false, Message = "Order not found.", Data = false };
                }

                order.Status = status;
                _orderRepository.Update(order);
                await _context.SaveChangesAsync();

                return new ResponseModel<bool> { IsSuccess = true, Message = "Order status updated successfully.", Data = true };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "Error updating status: " + ex.Message, Data = false };
            }
        }

        public async Task<ResponseModel<bool>> CancelOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ResponseModel<bool> { IsSuccess = false, Message = "Order not found.", Data = false };
                }

                if (order.Status != OrderStatus.Pending)
                {
                    return new ResponseModel<bool> { IsSuccess = false, Message = "Only pending orders can be cancelled.", Data = false };
                }

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

                return new ResponseModel<bool> { IsSuccess = true, Message = "Order cancelled successfully.", Data = true };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "Error cancelling order: " + ex.Message, Data = false };
            }
        }
    }
}
