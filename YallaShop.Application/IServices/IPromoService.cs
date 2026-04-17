using YallaShop.Application.DTOs.Promo;

namespace YallaShop.Application.IServices
{
    public interface IPromoService
    {
        Task<ResponseModel<PromoResultDto>> ApplyPromoAsync(ApplyPromoDto dto);
        Task<ResponseModel<PromoResultDto>> CreatePromoAsync(CreatePromoDto dto);
        Task<ResponseModel<bool>> DeletePromoAsync(int id);
        Task<ResponseModel<List<PromoResultDto>>> GetAllAsync();
    }
}
