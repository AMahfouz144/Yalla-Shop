using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface IAuthService
    {
        Task<ResponseModel<UserResponseDto>> RegisterAsync(RegisterDto request);
        Task<ResponseModel<UserResponseDto>> LoginAsync(LoginDto request);

        Task<ResponseModel<bool>> ConfirmEmailAsync(ConfirmEmailRequest request);

        Task<ResponseModel<bool>> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);

        Task<ResponseModel<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<ResponseModel<bool>> ResetPasswordAsync(string userId, string code, ResetPasswordRequest request);
    }
}
