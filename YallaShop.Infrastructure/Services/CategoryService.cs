using Microsoft.EntityFrameworkCore;
using YallaShop.Application.DTOs;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly AppDbContext _context;

        public CategoryService(IGenericRepository<Category> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync()
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null || category.IsDeleted) return null;
            return MapToDto(category);
        }

        public async Task<CategoryDto> CreateAsync(CategoryAddDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                CreatedAt = DateTime.Now
            };
            await _repository.AddAsync(category);
            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _repository.GetByIdAsync(dto.Id);
            if (category == null || category.IsDeleted) return null;

            category.Name = dto.Name;
            _repository.Update(category);
            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null || category.IsDeleted) return false;

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // Manual mapping: Category -> CategoryDto
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt
            };
        }


    }
}
