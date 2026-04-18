using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YallaShop.Application.IServices;
using YallaShop.API.ViewModels;
using AutoMapper;
using YallaShop.Application.DTOs;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public AuthController(RoleManager<IdentityRole> roleManager, IAuthService authService , IMapper mapper)
        {
            _roleManager = roleManager;
            _authService = authService;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel request)
        {  
            RegisterDto registerDto = _mapper.Map<RegisterDto>(request);
            var result = await _authService.RegisterAsync(registerDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request)
        {
            LoginDto loginDto = _mapper.Map<LoginDto>(request);
            var result = await _authService.LoginAsync(loginDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("confirm-email")]

        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel request)
        {
            var result = await _authService.ConfirmEmailAsync(_mapper.Map<ConfirmEmailRequest>(request));

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendEmailConfirmationViewModel request)
        {
            var dto = _mapper.Map<ResendEmailConfirmationRequest>(request);
            var result = await _authService.ResendEmailConfirmationAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel request)
        {
            var dto = _mapper.Map<ForgotPasswordRequest>(request);
            var result = await _authService.ForgotPasswordAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string userId , string code, [FromBody] ResetPasswordViewModel request)
        {
            var dto = _mapper.Map<ResetPasswordRequest>(request);
            var result = await _authService.ResetPasswordAsync(userId, code, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
