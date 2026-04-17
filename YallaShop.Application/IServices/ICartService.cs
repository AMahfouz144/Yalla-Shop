using YallaShop.Application.DTOs.Cart;

namespace YallaShop.Application.IServices
{
    public interface ICartService
    {
        Task<ResponseModel<CartDto>> GetCartAsync(GetCartDto dto);
        Task<ResponseModel<CartDto>> AddToCartAsync(AddToCartDto dto);
        Task<ResponseModel<CartDto>> UpdateCartItemAsync(UpdateCartItemDto dto);
        Task<ResponseModel<bool>> RemoveCartItemAsync(int cartItemId);
        Task<ResponseModel<bool>> ClearCartAsync(string? userId, string? guestSessionId);
    }
}
