using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface IReviewService
    {
        Task<ResponseModel<ReviewResponseDto>> AddReviewAsync(ReviewRequestDto request);
        Task<ResponseModel<bool>> UpdateReviewAsync(int id, ReviewRequestDto request);
        Task<ResponseModel<bool>> DeleteReviewAsync(int id);
        Task<ResponseModel<IEnumerable<ReviewResponseDto>>> GetAllProductReviewsAsync(int productId);
        Task<ResponseModel<double>> GetProductTotalRatingAsync(int productId);
    }
}
