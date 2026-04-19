using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<List<ProductDto>>GetProductsOfSeller(int sellerId);
        Task<ProductDto> CreateAsync(string imageUrl, ProductAddDto dto);
        Task<ProductDto?> UpdateAsync(string imageUrl, ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductDto>> FilterAsync(ProductFilterDto filter);
    }
}
