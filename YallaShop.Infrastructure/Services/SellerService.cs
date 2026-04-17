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
    public class SellerService(IGenericRepository<Seller> sellerRepository) : ISellerService
    {
        private readonly IGenericRepository<Seller> _sellerRepository = sellerRepository;

        public async Task<ResponseModel<SellerRequestDto>> CreateSellerAsync(SellerRequestDto request)
        {
            if (request == null)
            {
                return new ResponseModel<SellerRequestDto>
                {
                    IsSuccess = false,
                    Message = "Request cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return new ResponseModel<SellerRequestDto>
                {
                    IsSuccess = false,
                    Message = "UserId is required.",
                    Data = null
                };
            }

            var seller = new Seller
            {
                UserId = request.UserId,
                StoreName = request.StoreName
            };

            try
            {
                await _sellerRepository.AddAsync(seller);

                return new ResponseModel<SellerRequestDto>
                {
                    IsSuccess = true,
                    Message = "Seller created successfully.",
                    Data = new SellerRequestDto { UserId = seller.UserId, StoreName = seller.StoreName }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<SellerRequestDto>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

       public async Task<bool> GetSellerByIdAsync(int id)
        {
            var seller = await _sellerRepository.GetByIdAsync(id);
            return seller != null;
        }
    }
}
