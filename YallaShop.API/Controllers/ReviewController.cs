using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaShop.Application.IServices;
using AutoMapper;
using YallaShop.API.ViewModels;
using System.Linq;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateViewModel request)
        {
            var dto = _mapper.Map<Application.DTOs.ReviewRequestDto>(request);
            var result = await _reviewService.AddReviewAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            var vm = _mapper.Map<ReviewResponseViewModel>(result.Data);
            return Ok(new { result.IsSuccess, result.Message, Data = vm });
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUpdateViewModel request)
        {
            var dto = _mapper.Map<Application.DTOs.ReviewRequestDto>(request);
            var result = await _reviewService.UpdateReviewAsync(id, dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(new { result.IsSuccess, result.Message, Data = true });
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetAllProductReviews(int productId)
        {
            var result = await _reviewService.GetAllProductReviewsAsync(productId);
            if (!result.IsSuccess) return BadRequest(result);
            var vms = result.Data.Select(r => _mapper.Map<ReviewResponseViewModel>(r));
            return Ok(new { result.IsSuccess, result.Message, Data = vms });
        }

        [HttpGet("product/{productId}/rating")]
        public async Task<IActionResult> GetProductRating(int productId)
        {
            var result = await _reviewService.GetProductTotalRatingAsync(productId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
