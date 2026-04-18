using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application;
using YallaShop.Application.DTOs;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Infrastructure.Helpers;
using YallaShop.Infrastructure.Persistence;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YallaShop.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        AppDbContext context, IJwtService jwtService, ISellerService sellerService, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly AppDbContext _context = context;
        private readonly IJwtService _jwtService = jwtService;
        private readonly ISellerService _sellerService = sellerService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task<ResponseModel<UserResponseDto>> RegisterAsync(RegisterDto request)
        {
            try
            {
                // check if user already exists
                var existing = await _userManager.FindByNameAsync(request.UserName);
                if (existing != null)
                {
                    return new ResponseModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        Message = "User already exists.",
                        Data = null
                    };
                }

                var user = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.UserName,
                    FullName = request.FullName,
                    Address = request.Address,
                };

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    return new ResponseModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        Message = errors,
                        Data = null
                    };
                }
                var cart = new Cart
                {
                    UserId = user.Id
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                // assign the role if provided and exists
                if (!string.IsNullOrWhiteSpace(request.Role))
                {
                    if (await _roleManager.RoleExistsAsync(request.Role))
                    {
                        await _userManager.AddToRoleAsync(user, request.Role);
                    }
                }

                if (request.Role == "Seller")
                {
                    await _sellerService.CreateSellerAsync(new SellerRequestDto
                    {
                        UserId = user.Id,
                        StoreName = request.StoreName
                    });
                }
                //CONFIRMATION CODE
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                Console.WriteLine($"Confirmation Code : {code}");

                await SendConfirmationEmail(user, code);

                // generate token
                var (token, expiration) = await _jwtService.GenerateTokenAsync(user);


                return new ResponseModel<UserResponseDto>
                {
                    IsSuccess = true,
                    Message = "Registered successfully.",
                    Data = new UserResponseDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        token = token,
                        TokenExpiryTime = expiration,
                        FullName = user.FullName
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<UserResponseDto>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<UserResponseDto>> LoginAsync(LoginDto request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return new ResponseModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        Message = "Invalid username or password.",
                        Data = null
                    };
                }
                if(!user.EmailConfirmed)
                {
                    return new ResponseModel<UserResponseDto>
                    {
                        IsSuccess = false,
                        Message = "Email not confirmed. Please check your email for confirmation instructions.",
                        Data = null
                    };
                }
                var (token, expiration) = await _jwtService.GenerateTokenAsync(user);
                var responseDto = new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    token = token,
                    TokenExpiryTime = expiration,
                    FullName = user.FullName
                };
                return new ResponseModel<UserResponseDto>
                {
                    IsSuccess = true,
                    Message = "Logged in successfully.",
                    Data = responseDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<UserResponseDto>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EamilConfirmation",
               templateModel: new Dictionary<string, string>
               {
                   { "{{name}}" , $"{user.FullName}" },
                       {"{{action_url}}" , $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }

               });

            await _emailSender.SendEmailAsync(user.Email!, "YallaShop: Email Confirmation", emailBody);
        }

        public async Task<ResponseModel<bool>> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var response = new ResponseModel<bool>();
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
              { 
                response.IsSuccess = false;
                response.Message = "User not found.";
                response.Data = false;
                return response;
            }

            if (user.EmailConfirmed)
            {
                response.IsSuccess = true;
                response.Message = "Email already confirmed.";
                response.Data = true;
                return response;
            }

            var code = request.code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                response.IsSuccess = false;
                response.Message = "Invalid code.";
                response.Data = false;
                return response;
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Message = "Email confirmed successfully.";
                response.Data = true;
                return response;
            }

            response.IsSuccess = false;
            response.Message = "Email confirmation failed.";    
            response.Data = false;
            return response;
        }

        // Resend confirmation using username; returns generic success to avoid enumeration
        public async Task<ResponseModel<bool>> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request)
        {
            var response = new ResponseModel<bool> { IsSuccess = true, Message = "If the user exists, a confirmation email has been sent.", Data = true };

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return response;

            if (user.EmailConfirmed)
            {
                response.IsSuccess = false;
                response.Message = "Email already confirmed.";
                response.Data = false;
                return response;
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            await SendConfirmationEmail(user, code);

            return response;
        }

        // Forgot password: send reset link if user exists (generic response)
        public async Task<ResponseModel<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var response = new ResponseModel<bool> { IsSuccess = true, Message = "If the user exists, a password reset email has been sent.", Data = true };

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return response;

            // Optionally check if email confirmed; still return generic response
            //if (!user.EmailConfirmed) return response;

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            Console.WriteLine($"Password Reset Code : {code}");

            await SendPasswordResetEmail(user, code);
            return response;
        }

        public async Task<ResponseModel<bool>> ResetPasswordAsync(string userId , string code, ResetPasswordRequest request)
        {
            var response = new ResponseModel<bool>();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "User not found.";
                response.Data = false;
                return response;
            }

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                response.IsSuccess = false;
                response.Message = "Invalid code.";
                response.Data = false;
                return response;
            }

            var result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Message = "Password reset successful.";
                response.Data = true;
                return response;
            }

            response.IsSuccess = false;
            response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
            response.Data = false;
            return response;
        }

        private async Task SendPasswordResetEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("PasswordReset",
             templateModel: new Dictionary<string, string>
             {
             { "{{name}}" , $"{user.FullName}" },
             {"{{action_url}}" , $"{origin}/auth/reset-password?userId={user.Id}&code={code}" } }

            );

            await _emailSender.SendEmailAsync(user.Email!, "YallShop: Password Reset", emailBody);
        }
    }
}
