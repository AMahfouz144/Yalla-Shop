using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YallaShop.API.ViewModels;
using YallaShop.Application.IServices;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishistsController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly IMapper _mapper;

        public WishistsController(IWishlistService wishlistService, IMapper mapper)
        {
            _wishlistService = wishlistService;
            _mapper = mapper;
        }
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistCreateViewModel request)
        {
            var dto = _mapper.Map<Application.DTOs.WishlistRequestDto>(request);
            var result = await _wishlistService.AddProductToWishlistAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            var vm = _mapper.Map<WishlistResponseViewModel>(result.Data);
            return Ok(new { result.IsSuccess, result.Message, Data = vm });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFromWishlist(int id)
        {
            string UserId = GetUserId();
            var result = await _wishlistService.DeleteProductFromWishlistAsync(UserId,id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _wishlistService.GetAllProductsForUserAsync(userId);
            if (!result.IsSuccess) return BadRequest(result);
            var vms = result.Data.Select(i => _mapper.Map<WishlistResponseViewModel>(i));
            return Ok(new { result.IsSuccess, result.Message, Data = vms });
        }
    }
}
