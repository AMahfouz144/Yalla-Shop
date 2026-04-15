using Microsoft.EntityFrameworkCore;
using YallaShop.Application;
using YallaShop.Application.DTOs.Promo;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;

namespace YallaShop.Infrastructure.Services
{
    public class PromoService : IPromoService
    {
        private readonly IGenericRepository<PromoCode> _promoRepo;

        public PromoService(IGenericRepository<PromoCode> promoRepo)
        {
            _promoRepo = promoRepo;
        }

        public async Task<ResponseModel<PromoResultDto>> ApplyPromoAsync(ApplyPromoDto dto)
        {
            var normalizedCode = dto.Code.ToUpper().Trim();
            var promo = await _promoRepo.GetAllAsync()
                .FirstOrDefaultAsync(p => p.Code == normalizedCode && !p.IsDeleted);

            if (promo == null)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = "Invalid promo code" };
            if (!promo.IsActive)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = "Code is not active" };
            if (DateTime.UtcNow < promo.StartDate)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = "Code not valid yet" };
            if (DateTime.UtcNow > promo.EndDate)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = "Code has expired" };
            if (promo.MaxUsageCount.HasValue && promo.UsedCount >= promo.MaxUsageCount)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = "Code reached its limit" };
            if (promo.MinOrderAmount.HasValue && dto.CartSubTotal < promo.MinOrderAmount)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = $"Minimum order is {promo.MinOrderAmount}" };

            var discount = promo.DiscountType == DiscountType.Percentage
                ? dto.CartSubTotal * (promo.DiscountValue / 100m)
                : promo.DiscountValue;

            return new ResponseModel<PromoResultDto>
            {
                IsSuccess = true,
                Message = $"Code applied! You save {discount:F2}",
                Data = new PromoResultDto
                {
                    Code = promo.Code,
                    Discount = discount,
                    Message = $"You save {discount:F2}"
                }
            };
        }

        public async Task<ResponseModel<PromoResultDto>> CreatePromoAsync(CreatePromoDto dto)
        {
            var normalizedCode = dto.Code.ToUpper().Trim();
            var exists = await _promoRepo.GetAllAsync()
                .AnyAsync(p => p.Code == normalizedCode && !p.IsDeleted);
            if (exists)
                return new ResponseModel<PromoResultDto> { IsSuccess = false, Message = "Code already exists" };

            var promo = new PromoCode
            {
                Code = normalizedCode,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxUsageCount = dto.MaxUsageCount,
                UsedCount = 0,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true
            };
            await _promoRepo.AddAsync(promo);
            await _promoRepo.SaveChangesAsync();

            return new ResponseModel<PromoResultDto>
            {
                IsSuccess = true,
                Message = "Promo created",
                Data = new PromoResultDto { Code = promo.Code }
            };
        }

        public async Task<ResponseModel<bool>> DeletePromoAsync(int id)
        {
            var result = await _promoRepo.DeleteAsync(id);
            if (!result)
                return new ResponseModel<bool> { IsSuccess = false, Message = "Promo not found", Data = false };
            return new ResponseModel<bool> { IsSuccess = true, Message = "Deleted", Data = true };
        }

        public async Task<ResponseModel<List<PromoResultDto>>> GetAllAsync()
        {
            var list = await _promoRepo.GetAllAsync()
                .Where(p => !p.IsDeleted)
                .Select(p => new PromoResultDto
                {
                    Code = p.Code,
                    Discount = p.DiscountValue,
                    Message = p.IsActive ? "Active" : "Inactive"
                }).ToListAsync();

            return new ResponseModel<List<PromoResultDto>>
            {
                IsSuccess = true,
                Message = "Promo list loaded",
                Data = list
            };
        }
    }
}
