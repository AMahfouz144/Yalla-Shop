using AutoMapper;
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
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ResponseModel<ReviewResponseDto>> AddReviewAsync(string userId , ReviewRequestDto request)
        {
            try
            {
                var response = new ResponseModel<ReviewResponseDto>();
                // validate rating
                if (request.Rating < 0 || request.Rating > 5)
                {
                    response.IsSuccess = false;
                    response.Message = "Rating must be between 0 and 5.";
                    response.Data = null;
                    return response;
                }

                bool alreadyReviewed = await _reviewRepository.GetAllAsync()
                    .AnyAsync(r => r.UserId == userId && r.ProductId == request.ProductId && r.IsDeleted == false);

                if (alreadyReviewed)
                {
                    response.IsSuccess = false;
                    response.Message = "You have already reviewed this product.";
                    response.Data = null;
                    return response;
                }

                var review = _mapper.Map<Review>(request);
                review.UserId = userId;
                await _reviewRepository.AddAsync(review);

                response.IsSuccess = true;
                response.Message = "Review added.";
                response.Data = _mapper.Map<ReviewResponseDto>(review);
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<ReviewResponseDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<bool>> UpdateReviewAsync(int id, ReviewRequestDto request)
        {
            try
            {
                var response = new ResponseModel<bool>();
                if (request.Rating < 0 || request.Rating > 5)
                {
                    response.IsSuccess = false;
                    response.Message = "Rating must be between 0 and 5.";
                    response.Data = false;
                    return response;
                }

                var existing = await _reviewRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Review not found.";
                    response.Data = false;
                    return response;
                }

                existing.Rating = request.Rating;
                existing.Comment = request.Comment;

                _reviewRepository.Update(existing);

                response.IsSuccess = true;
                response.Message = "Review updated.";
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

        public async Task<ResponseModel<bool>> DeleteReviewAsync(int id)
        {
            try
            {
                var response = new ResponseModel<bool>();
                var existing = await _reviewRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Review not found.";
                    response.Data = false;
                    return response;
                }

                var deleted = await _reviewRepository.DeleteAsync(id);
                response.IsSuccess = deleted;
                response.Message = deleted ? "Deleted successfully." : "Delete failed.";
                response.Data = deleted;
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

        public async Task<ResponseModel<IEnumerable<ReviewResponseDto>>> GetAllProductReviewsAsync(int productId)
        {
            try
            {
                var response = new ResponseModel<IEnumerable<ReviewResponseDto>>();
                var reviews =  _reviewRepository.GetAllByProductId(productId);
                var dtoList = reviews.Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    ReviewerName = r.User.FullName,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList();
                response.IsSuccess = true;
                response.Message = "Reviews Loaded Successfully.";
                response.Data = dtoList;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<ReviewResponseDto>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<double>> GetProductTotalRatingAsync(int productId)
        {
            try
            {
                var response = new ResponseModel<double>();
                var ratings = await _reviewRepository.GetAllByProductId(productId).Select(r => r.Rating).ToListAsync();
                if (!ratings.Any())
                {
                    response.IsSuccess = true;
                    response.Message = "No Reviews";
                    response.Data = 0;
                    return response;
                }

                var avg = ratings.Average();
                response.IsSuccess = true;
                response.Message = "Success.";
                response.Data = avg;
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseModel<double>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = 0
                };
            }
        }
    }
}
