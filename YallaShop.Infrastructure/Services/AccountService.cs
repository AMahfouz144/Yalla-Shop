using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
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

namespace YallaShop.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<UserProfileDto>> GetProfileAsync(string userId)
        {
            try
            {
                var response = new ResponseModel<UserProfileDto>();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    response.Data = null;
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "Profile retrieved.";
                response.Data = new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber
                };
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<UserProfileDto>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<UserProfileDto>> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            try
            {
                var response = new ResponseModel<UserProfileDto>();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    response.Data = null;
                    return response;
                }

                user.FullName = request.FullName ?? user.FullName;
                user.Address = request.Address;
                user.PhoneNumber = request.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
                    response.Data = null;
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "Profile updated successfully.";
                response.Data = new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber
                };
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<UserProfileDto>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            try
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

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
                    response.Data = false;
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "Password changed successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<bool>> UpdateEmailAsync(string userId, UpdateEmailRequest request)
        {
            try
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

                // update email: generate change email token and send confirmation to new email
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
                var url = $"{origin}/auth/confirm-change-email?userId={user.Id}&email={request.NewEmail}&code={token}";

                var body = EmailBodyBuilder.GenerateEmailBody("EamilConfirmation", new Dictionary<string, string>
                {
                    { "{{name}}", user.FullName },
                    { "{{action_url}}", url }
                });

                await _emailSender.SendEmailAsync(request.NewEmail, "YallaShop: Confirm Email Change", body);

                response.IsSuccess = true;
                response.Message = "Confirmation email sent to new email address.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<bool>> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request)
        {
            try
            {
                var response = new ResponseModel<bool>();
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    response.Data = false;
                    return response;
                }

                var token = request.EmailChangeToken;
                try
                {
                    token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                }
                catch (FormatException)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid token.";
                    response.Data = false;
                    return response;
                }

                var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, token);
                if (!result.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
                    response.Data = false;
                    return response;
                }

                // optionally update user.UserName if you use email as username
                user.UserName = request.NewEmail;
                await _userManager.UpdateAsync(user);

                response.IsSuccess = true;
                response.Message = "Email changed successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
