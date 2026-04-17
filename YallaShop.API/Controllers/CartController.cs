using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YallaShop.API.ViewModels.Cart;
using YallaShop.Application.DTOs.Cart;
using YallaShop.Application.IServices;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }

        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private string? GetOrCreateGuestSession()
        {
            if (GetUserId() != null)
            {
                return null;
            }

            if (Request.Cookies.TryGetValue("guestSession", out var sessionId))
            {
                return sessionId;
            }

            sessionId = Guid.NewGuid().ToString();
            Response.Cookies.Append("guestSession", sessionId, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });

            return sessionId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await _cartService.GetCartAsync(new GetCartDto
            {
                UserId = GetUserId(),
                GuestSessionId = GetOrCreateGuestSession()
            });
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            var vm = _mapper.Map<CartResponseVm>(result.Data!);
            return Ok(new { result.IsSuccess, result.Message, Data = vm });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequestVm request)
        {
            var dto = _mapper.Map<AddToCartDto>(request);
            dto.UserId = GetUserId();
            dto.GuestSessionId = GetOrCreateGuestSession();

            var result = await _cartService.AddToCartAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            var vm = _mapper.Map<CartResponseVm>(result.Data!);
            return Ok(new { result.IsSuccess, result.Message, Data = vm });
        }

        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemRequestVm request)
        {
            var dto = _mapper.Map<UpdateCartItemDto>(request);
            dto.CartItemId = cartItemId;
            dto.UserId = GetUserId();
            dto.GuestSessionId = GetOrCreateGuestSession();
            var result = await _cartService.UpdateCartItemAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            var vm = _mapper.Map<CartResponseVm>(result.Data!);
            return Ok(new { result.IsSuccess, result.Message, Data = vm });
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(cartItemId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync(GetUserId(), GetOrCreateGuestSession());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
