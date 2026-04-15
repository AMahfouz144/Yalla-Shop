using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryAddDto dto);
        Task<CategoryDto?> UpdateAsync(CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
