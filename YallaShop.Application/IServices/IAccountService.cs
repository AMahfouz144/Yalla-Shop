using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface IAccountService
    {
        Task<ResponseModel<UserProfileDto>> GetProfileAsync(string userId);
        Task<ResponseModel<UserProfileDto>> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<ResponseModel<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<ResponseModel<bool>> UpdateEmailAsync(string userId, UpdateEmailRequest request);
        Task<ResponseModel<bool>> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request);
    }
}
