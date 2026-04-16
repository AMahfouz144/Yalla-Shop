using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductAddDto dto);
        Task<ProductDto?> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductDto>> FilterAsync(ProductFilterDto filter);
    }
}
