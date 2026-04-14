using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryDto dto);
        Task<CategoryDto?> UpdateAsync(CategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
