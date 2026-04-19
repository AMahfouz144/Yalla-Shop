using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs.User;

namespace YallaShop.Application.IServices
{
    public interface IAdminService
    {
         public Task<ResponseModel<IEnumerable<UserResponse>>> GetAllCustomersAsync();
         public Task<ResponseModel<IEnumerable<UserResponse>>> GetAllSellersAsync();
         public Task<ResponseModel<UserResponse>> GetUserByIdAsync(string id);
         public Task<ResponseModel<bool>>  DeleteUserAsync(string id);
         public Task<ResponseModel<bool>> ToggleProductStatus(int ProductId , int AcceptionStatus);
         public Task<ResponseModel<IEnumerable<ProductResponseDto>>> GetAllProductsAsync();

    }
}
