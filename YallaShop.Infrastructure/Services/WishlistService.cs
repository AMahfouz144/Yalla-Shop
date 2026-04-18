using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application;
using YallaShop.Application.DTOs;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;

namespace YallaShop.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<ResponseModel<WishlistResponseDto>> AddProductToWishlistAsync(WishlistRequestDto request)
        {
            try
            {
                var response = new ResponseModel<WishlistResponseDto>();
                // check existing duplicate for same user and product
                var existing = await _wishlistRepository.GetAllAsync()
                    .FirstOrDefaultAsync(w => w.UserId == request.UserId && w.ProductId == request.ProductId && !w.IsDeleted);
                if (existing != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product already in wishlist.";
                    response.Data = null;
                    return response;
                }

                var wishlist = new Wishlist
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId
                };
                await _wishlistRepository.AddAsync(wishlist);
                Console.WriteLine($"Wishlist item added with ID: {wishlist.Id}");
                // reload including product navigation

                response.IsSuccess = true;
                response.Message = "Added to wishlist.";
                response.Data = await _wishlistRepository.GetAllAsync()
                    .Where(w => w.Id == wishlist.Id)
                    .Select(w => new WishlistResponseDto
                    {
                        Id = w.Id,
                        UserId = w.UserId,
                        ProductId = w.ProductId,
                        CreatedAt = w.CreatedAt,
                        Product = w.Product == null ? null : new ProductResponseDto
                        {
                            Id = w.Product.Id,
                            Name = w.Product.Name,
                            Description = w.Product.Description,
                            Price = w.Product.Price,
                            StockQuantity = w.Product.StockQuantity,
                            Picture = w.Product.Picture
                        }
                    })
                    .FirstOrDefaultAsync();
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<WishlistResponseDto>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteProductFromWishlistAsync(string userId , int productId)
        {
            try
            {
                var response = new ResponseModel<bool>();
                Wishlist WishlistItem = await _wishlistRepository.GetByUserIdAndProductIdAsync(userId, productId);
                bool deleted = await _wishlistRepository.DeleteAsync(WishlistItem.Id);
                response.IsSuccess = deleted;
                response.Message = deleted ? "Deleted." : "Delete failed.";
                response.Data = deleted;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<WishlistResponseDto>>> GetAllProductsForUserAsync(string userId)
        {
            try
            {
                var response = new ResponseModel<IEnumerable<WishlistResponseDto>>();
                var items = await _wishlistRepository.GetAllAsync()
                    .Where(w => w.UserId == userId && !w.IsDeleted)
                .Select(i => new WishlistResponseDto
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    ProductId = i.ProductId,
                    CreatedAt = i.CreatedAt,
                    Product = i.Product == null ? null : new ProductResponseDto
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Description = i.Product.Description,
                        Price = i.Product.Price,
                        StockQuantity = i.Product.StockQuantity,
                        Picture = i.Product.Picture
                    }
                }).ToListAsync();
                response.IsSuccess = true;
                response.Message = "Success.";
                response.Data = items;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<WishlistResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
