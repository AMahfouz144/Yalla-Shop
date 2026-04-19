using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YallaShop.Application.DTOs;
using YallaShop.Application.IServices;
using YallaShop.Domain.Enums;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Temporarily disabled until Authentication Scheme is configured
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _orderService.GetAllOrdersAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var result = await _orderService.GetOrderStatusAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
