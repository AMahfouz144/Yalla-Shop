using YallaShop.Application.DTOs.Checkout;

namespace YallaShop.Application.IServices
{
    public interface ICheckoutService
    {
        Task<ResponseModel<OrderDto>> CheckoutAsync(CheckoutDto dto);
    }
}
