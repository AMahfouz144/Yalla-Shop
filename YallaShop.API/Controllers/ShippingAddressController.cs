using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaShop.Application.DTOs.ShippingAddress;
using YallaShop.Application.IServices;

namespace YallaShop.API.Controllers
{
    [ApiController]
    [Route("api/shipping-address")]
    [Authorize]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _service;

        public ShippingAddressController(IShippingAddressService service)
        {
            _service = service;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync(GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShippingAddressDto dto)
        {
            var result = await _service.CreateAsync(GetUserId(), dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateShippingAddressDto dto)
        {
            var result = await _service.UpdateAsync(id, GetUserId(), dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var result = await _service.SetDefaultAsync(id, GetUserId());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
