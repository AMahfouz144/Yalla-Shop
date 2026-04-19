using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;
using YallaShop.Domain.Enums;

namespace YallaShop.Application.IServices
{
    public interface IOrderService
    {
        Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync(string userId);
        Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerId);
        Task<ResponseModel<IEnumerable<OrderResponseDto>>> GetOrdersBySellerAsync(string sellerId);
        Task<ResponseModel<OrderResponseDto>> GetOrderByIdAsync(int orderId);
        Task<ResponseModel<OrderStatus>> GetOrderStatusAsync(int orderId);
        Task<ResponseModel<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<ResponseModel<bool>> CancelOrderAsync(int orderId);
    }
}
