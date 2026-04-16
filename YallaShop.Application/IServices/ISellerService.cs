using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;

namespace YallaShop.Application.IServices
{
    public interface ISellerService
    {
        public Task<bool> GetSellerByIdAsync(int id);
        public Task<ResponseModel<SellerRequestDto>> CreateSellerAsync(SellerRequestDto request);
    }
}
