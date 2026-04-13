using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface IWishlistService
    {
        Task<ResponseModel<WishlistResponseDto>> AddProductToWishlistAsync(WishlistRequestDto request);
        Task<ResponseModel<bool>> DeleteProductFromWishlistAsync(string userId,int id);
        Task<ResponseModel<IEnumerable<WishlistResponseDto>>> GetAllProductsForUserAsync(string userId);
    }
}
