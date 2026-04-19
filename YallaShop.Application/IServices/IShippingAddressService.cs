using YallaShop.Application.DTOs.ShippingAddress;

namespace YallaShop.Application.IServices
{
    public interface IShippingAddressService
    {
        Task<ResponseModel<List<ShippingAddressDto>>> GetAllAsync(string userId);
        Task<ResponseModel<ShippingAddressDto>> GetByIdAsync(int id, string userId);
        Task<ResponseModel<ShippingAddressDto>> CreateAsync(string userId, CreateShippingAddressDto dto);
        Task<ResponseModel<ShippingAddressDto>> UpdateAsync(int id, string userId, CreateShippingAddressDto dto);
        Task<ResponseModel<bool>> DeleteAsync(int id, string userId);
        Task<ResponseModel<bool>> SetDefaultAsync(int id, string userId);
    }
}
