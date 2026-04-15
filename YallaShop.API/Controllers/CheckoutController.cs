using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YallaShop.API.ViewModels.Checkout;
using YallaShop.Application.DTOs.Checkout;
using YallaShop.Application.IServices;

namespace YallaShop.API.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        //try
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutVM vm)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var dto = new CheckoutDto
            {
                UserId = userId,
                GuestEmail = vm.GuestEmail,
                GuestSessionId = userId == null ? Request.Cookies["guestSession"] : null,
                ShippingAddressId = vm.ShippingAddressId,
                PaymentMethod = vm.PaymentMethod,
                PromoCode = vm.PromoCode
            };
            var result = await _checkoutService.CheckoutAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
