using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using YallaShop.API.ViewModels.User;
using YallaShop.API.ViewModels;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _adminService.GetAllCustomersAsync();
            if (!result.IsSuccess) return BadRequest(result);
            var vms = result.Data.Select(u => _mapper.Map<YallaShop.API.ViewModels.User.CustomerResponseViewModel>(u));
            return Ok(new { result.IsSuccess, result.Message, Data = vms });
        }

        [HttpGet("sellers")]
        public async Task<IActionResult> GetAllSellers()
        {
            var result = await _adminService.GetAllSellersAsync();
            if (!result.IsSuccess) return BadRequest(result);
            var vms = result.Data.Select(u => _mapper.Map<YallaShop.API.ViewModels.SellerResponseViewModel>(u));
            return Ok(new { result.IsSuccess, result.Message, Data = vms });
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _adminService.GetAllProductsAsync();
            if (!result.IsSuccess) return BadRequest(result);
            var vms = result.Data.Select(p => _mapper.Map<YallaShop.API.ViewModels.User.ProductResponseViewModel>(p));
            return Ok(new { result.IsSuccess, result.Message, Data = vms });
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _adminService.GetUserByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result);
            var vm = _mapper.Map<YallaShop.API.ViewModels.User.CustomerResponseViewModel>(result.Data);
            return Ok(new { result.IsSuccess, result.Message, Data = vm });
        }

        [HttpDelete("user/{id}/toggleStatus")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("product/{productId}/status/{status}")]
        public async Task<IActionResult> ToggleProductStatus(int productId, int status)
        {
            var result = await _adminService.ToggleProductStatus(productId, status);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
