using Microsoft.EntityFrameworkCore;
using YallaShop.Application.DTOs;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly AppDbContext _context;

        public ProductService(IGenericRepository<Product> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await ActiveProductsInActiveCategories()
                .Include(p => p.Category)
                .ToListAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await ActiveProductsInActiveCategories()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;
            return MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(string imageUrl, ProductAddDto dto)
        {
            byte[]? picture = null;
            if (dto.Image != null)
            {
                using var memoryStream = new MemoryStream();
                await dto.Image.CopyToAsync(memoryStream);
                picture = memoryStream.ToArray();
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = imageUrl,
                CategoryId = dto.CategoryId,
                SellerId = dto.SellerId,
                CreatedAt = DateTime.Now,
                Status = ProductStatus.Pending
            };
            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Reload with Category for the response
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateAsync(string imageUrl , ProductUpdateDto dto)
        {
            var product = await ActiveProductsInActiveCategories()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);
            if (product == null) return null;

           

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.Status = dto.Status;
            product.CategoryId = dto.CategoryId;
            product.SellerId = dto.SellerId;
            product.ImageUrl = imageUrl;

            _repository.Update(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null || product.IsDeleted) return false;

            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDto>> FilterAsync(ProductFilterDto filter)
        {
            filter ??= new ProductFilterDto();

            var query = ActiveProductsInActiveCategories()
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(p => p.Name != null && p.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Description))
                query = query.Where(p => p.Description != null && p.Description.Contains(filter.Description));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.StockQuantity.HasValue)
                query = query.Where(p => p.StockQuantity >= filter.StockQuantity.Value);

            if (!string.IsNullOrWhiteSpace(filter.CategoryName))
                query = query.Where(p => p.Category != null && p.Category.Name.Contains(filter.CategoryName));

            if (filter.CreatedFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                query = query.Where(p => p.CreatedAt <= filter.CreatedTo.Value);

            // Sorting
            bool descending = filter.SortOrder?.ToLower() == "desc";

            query = filter.SortBy?.ToLower() switch
            {
                "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "date" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "stockquantity" => descending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
                _ => query.OrderBy(p => p.Id)
            };

            var products = await query.ToListAsync();
            return products.Select(MapToDto);
        }

        /** Product not soft-deleted and its category exists and is not soft-deleted. */
        private IQueryable<Product> ActiveProductsInActiveCategories()
        {
            return _repository.GetAllAsync()
                .Where(p => !p.IsDeleted && p.Status == ProductStatus.Accepted
                    && _context.Categories.Any(c => c.Id == p.CategoryId && !c.IsDeleted));
        }

        // Manual mapping: Product -> ProductDto
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                Status = product.Status,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CreatedAt = product.CreatedAt
            };
        }


    }
}
