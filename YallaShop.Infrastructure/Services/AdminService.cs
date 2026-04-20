using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application;
using YallaShop.Application.DTOs.User;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly AppDbContext _context;

        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IGenericRepository<Product> productRepository, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<ResponseModel<bool>> DeleteUserAsync(string id)
        {
            try
            {
                var response = new ResponseModel<bool>();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    response.Data = false;
                    return response;
                }

                user.IsDeleted = !user.IsDeleted;
                var result = await _userManager.UpdateAsync(user);
                response.IsSuccess = result.Succeeded;
                response.Message = result.Succeeded ? "User status toggled." : string.Join("; ", result.Errors.Select(e => e.Description));
                response.Data = result.Succeeded;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<UserResponse>>> GetAllCustomersAsync()
        {
            try
            {
                var response = new ResponseModel<IEnumerable<UserResponse>>();

                if (!await _roleManager.RoleExistsAsync("Customer"))
                {
                    response.IsSuccess = true;
                    response.Message = "Role 'Customer' does not exist.";
                    response.Data = Enumerable.Empty<UserResponse>();
                    return response;
                }

                var users = await _userManager.GetUsersInRoleAsync("Customer");
                var dto = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    IsDeleted = u.IsDeleted
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Customers loaded.";
                response.Data = dto;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<UserResponse>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<UserResponse>> GetUserByIdAsync(string id)
        {
            try
            {
                var response = new ResponseModel<UserResponse>();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    response.Data = null;
                    return response;
                }

                response.IsSuccess = true;
                response.Message = "User loaded.";
                response.Data = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    IsDeleted = user.IsDeleted
                };
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<UserResponse>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<bool>> ToggleProductStatus(int ProductId, int AcceptionStatus)
        {
            try
            {
                var response = new ResponseModel<bool>();
                var product = await _productRepository.GetByIdAsync(ProductId);
                if (product == null || product.IsDeleted)
                {
                    response.IsSuccess = false;
                    response.Message = "Product not found.";
                    response.Data = false;
                    return response;
                }

                // Validate provided status value
                if (!Enum.IsDefined(typeof(ProductStatus), AcceptionStatus))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid status value.";
                    response.Data = false;
                    return response;
                }

                product.Status = (ProductStatus)AcceptionStatus;
                _productRepository.Update(product);
                await _context.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "Product status updated.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<UserResponse>>> GetAllSellersAsync()
        {
            try
            {
                var response = new ResponseModel<IEnumerable<UserResponse>>();

                if (!await _roleManager.RoleExistsAsync("Seller"))
                {
                    response.IsSuccess = true;
                    response.Message = "Role 'Seller' does not exist.";
                    response.Data = Enumerable.Empty<UserResponse>();
                    return response;
                }

                var users = await _userManager.GetUsersInRoleAsync("Seller");
                var dto = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    IsDeleted = u.IsDeleted
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Sellers loaded.";
                response.Data = dto;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<UserResponse>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<ProductResponseDto>>> GetAllProductsAsync()
        {
            try
            {
                var response = new ResponseModel<IEnumerable<ProductResponseDto>>();
                var products = await _productRepository.GetAllAsync()
                    .Where(p => !p.IsDeleted)
                    .ToListAsync();

                var dto = products.Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    Status = p.Status,
                    CategoryId = p.CategoryId,
                    SellerId = p.SellerId
                }).ToList();

                response.IsSuccess = true;
                response.Message = "Products loaded.";
                response.Data = dto;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<ProductResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
