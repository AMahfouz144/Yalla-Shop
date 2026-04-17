using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YallaShop.API.ViewModels.Promo;
using YallaShop.Application.DTOs.Cart;
using YallaShop.Application.DTOs.Promo;
using YallaShop.Application.IServices;
using YallaShop.Domain.Enums;

namespace YallaShop.API.Controllers
{
    [ApiController]
    [Route("api/promo")]
    public class PromoController : ControllerBase
    {
        private readonly IPromoService _promoService;
        private readonly ICartService _cartService;

        public PromoController(IPromoService promoService, ICartService cartService)
        {
            _promoService = promoService;
            _cartService = cartService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyPromo([FromBody] ApplyPromoVM vm)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Request.Cookies.TryGetValue("guestSession", out var sessionId);

            var cart = await _cartService.GetCartAsync(
                new GetCartDto { UserId = userId, GuestSessionId = sessionId });

            var dto = new ApplyPromoDto
            {
                Code = vm.Code,
                CartSubTotal = cart.Data?.Subtotal ?? 0
            };
            var result = await _promoService.ApplyPromoAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePromo([FromBody] CreatePromoVM vm)
        {
            if (!Enum.TryParse<DiscountType>(vm.DiscountType, true, out var discountType))
            {
                return BadRequest(new { IsSuccess = false, Message = "Invalid discount type" });
            }

            var dto = new CreatePromoDto
            {
                Code = vm.Code,
                DiscountType = discountType,
                DiscountValue = vm.DiscountValue,
                MinOrderAmount = vm.MinOrderAmount,
                MaxUsageCount = vm.MaxUsageCount,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate
            };

            var result = await _promoService.CreatePromoAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePromo(int id)
        {
            var result = await _promoService.DeletePromoAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
    }
}
