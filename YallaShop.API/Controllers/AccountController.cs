using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YallaShop.Application.IServices;
using AutoMapper;
using YallaShop.API.ViewModels;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var result = await _accountService.GetProfileAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel request)
        {
            var userId = GetUserId();
            var dto = _mapper.Map<Application.DTOs.UpdateProfileRequest>(request);
            var result = await _accountService.UpdateProfileAsync(userId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel request)
        {
            var userId = GetUserId();
            var dto = _mapper.Map<Application.DTOs.ChangePasswordRequest>(request);
            var result = await _accountService.ChangePasswordAsync(userId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailViewModel request)
        {
            var userId = GetUserId();
            var dto = _mapper.Map<Application.DTOs.UpdateEmailRequest>(request);
            var result = await _accountService.UpdateEmailAsync(userId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("confirm-change-email")]
        public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmChangeEmailViewModel request)
        {
            var dto = _mapper.Map<Application.DTOs.ConfirmChangeEmailRequest>(request);
            var result = await _accountService.ConfirmChangeEmailAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
