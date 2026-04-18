using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application;
using YallaShop.Application.DTOs;
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

        public async Task<ResponseModel<int>> PlaceOrderAsync(string userId, PlaceOrderDto orderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Fetch Cart with Items and Products
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.Id == orderDto.CartId);

                if (cart == null)
                {
                    return new ResponseModel<int> { IsSuccess = false, Message = "Cart not found." };
                }

                if (cart.UserId != userId)
                {
                    return new ResponseModel<int> { IsSuccess = false, Message = "Unauthorized: This cart does not belong to you." };
                }

                if (cart.CartItems == null || !cart.CartItems.Any())
                {
                    return new ResponseModel<int> { IsSuccess = false, Message = "Cart is empty." };
                }

                // 2. Initialize Order
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.Now,
                    Street = orderDto.Street,
                    City = orderDto.City,
                    State = orderDto.State,
                    Country = orderDto.Country,
                    ZipCode = orderDto.ZipCode,
                    PaymentMethod = orderDto.PaymentMethod,
                    IsPaid = false,
                    Items = new List<OrderItem>()
                };

                decimal totalOrderPrice = 0;

                // 3. Process CartItems
                foreach (var cartItem in cart.CartItems)
                {
                    var product = cartItem.Product;
                    if (product == null)
                    {
                        return new ResponseModel<int> { IsSuccess = false, Message = $"Product details not found for item ID {cartItem.Id}." };
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        return new ResponseModel<int> { IsSuccess = false, Message = $"Insufficient stock for product {product.Name} (Available: {product.StockQuantity})." };
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        Price = product.Price, // Use server-side price
                        CreatedAt = DateTime.Now
                    };

                    totalOrderPrice += (orderItem.Price * orderItem.Quantity);
                    order.Items.Add(orderItem);

                    // Update stock
                    product.StockQuantity -= cartItem.Quantity;
                    _context.Products.Update(product);
                }

                order.TotalPrice = totalOrderPrice;

                // 4. Save Order
                await _context.Orders.AddAsync(order);

                // 5. Clear the Cart Items
                _context.CartItems.RemoveRange(cart.CartItems);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseModel<int>
                {
                    IsSuccess = true,
                    Message = "Order placed successfully and cart cleared.",
                    Data = order.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseModel<int>
                {
                    IsSuccess = false,
                    Message = "An error occurred while placing the order: " + ex.Message
                };
            }
        }
        public async Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var orderDtos = orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    Status = o.Status,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    Street = o.Street,
                    City = o.City,
                    State = o.State,
                    Country = o.Country,
                    ZipCode = o.ZipCode,
                    PaymentMethod = o.PaymentMethod,
                    IsPaid = o.IsPaid,
                    Items = o.Items.Select(i => new OrderItemResponseDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? "Unknown Product",
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                });

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
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new ResponseModel<OrderResponseDto> { IsSuccess = false, Message = "Order not found." };
                }

                var orderDto = new OrderResponseDto
                {
                    Id = order.Id,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    Street = order.Street,
                    City = order.City,
                    State = order.State,
                    Country = order.Country,
                    ZipCode = order.ZipCode,
                    PaymentMethod = order.PaymentMethod,
                    IsPaid = order.IsPaid,
                    Items = order.Items.Select(i => new OrderItemResponseDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? "Unknown Product",
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                return new ResponseModel<OrderResponseDto>
                {
                    IsSuccess = true,
                    Data = orderDto
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

                // Restore stock
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
