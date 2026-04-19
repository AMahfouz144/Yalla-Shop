using Microsoft.EntityFrameworkCore;
using YallaShop.Application;
using YallaShop.Application.DTOs.ShippingAddress;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;

namespace YallaShop.Infrastructure.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly IGenericRepository<ShippingAddress> _repo;

        public ShippingAddressService(IGenericRepository<ShippingAddress> repo)
        {
            _repo = repo;
        }

        public async Task<ResponseModel<List<ShippingAddressDto>>> GetAllAsync(string userId)
        {
            var entities = await _repo.GetAllAsync()
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .ToListAsync();
            var list = entities.Select(MapToDto).ToList();
            return new ResponseModel<List<ShippingAddressDto>> { IsSuccess = true, Data = list };
        }

        public async Task<ResponseModel<ShippingAddressDto>> GetByIdAsync(int id, string userId)
        {
            var address = await _repo.GetByIdAsync(id);
            if (address == null || address.UserId != userId || address.IsDeleted)
            {
                return new ResponseModel<ShippingAddressDto> { IsSuccess = false, Message = "Address not found" };
            }

            return new ResponseModel<ShippingAddressDto> { IsSuccess = true, Data = MapToDto(address) };
        }

        public async Task<ResponseModel<ShippingAddressDto>> CreateAsync(string userId, CreateShippingAddressDto dto)
        {
            if (dto.IsDefault)
            {
                var existing = await _repo.GetAllAsync()
                    .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted)
                    .ToListAsync();
                foreach (var a in existing)
                {
                    a.IsDefault = false;
                    _repo.Update(a);
                }
            }

            var address = new ShippingAddress
            {
                UserId = userId,
                Label = dto.Label,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                ZipCode = dto.ZipCode,
                IsDefault = dto.IsDefault
            };
            await _repo.AddAsync(address);
            return new ResponseModel<ShippingAddressDto>
            {
                IsSuccess = true,
                Message = "Address created",
                Data = MapToDto(address)
            };
        }

        public async Task<ResponseModel<ShippingAddressDto>> UpdateAsync(int id, string userId, CreateShippingAddressDto dto)
        {
            var address = await _repo.GetByIdAsync(id);
            if (address == null || address.UserId != userId || address.IsDeleted)
            {
                return new ResponseModel<ShippingAddressDto> { IsSuccess = false, Message = "Address not found" };
            }

            if (dto.IsDefault)
            {
                var existing = await _repo.GetAllAsync()
                    .Where(a => a.UserId == userId && a.IsDefault && a.Id != id && !a.IsDeleted)
                    .ToListAsync();
                foreach (var a in existing)
                {
                    a.IsDefault = false;
                    _repo.Update(a);
                }
            }

            address.Label = dto.Label;
            address.Street = dto.Street;
            address.City = dto.City;
            address.State = dto.State;
            address.Country = dto.Country;
            address.ZipCode = dto.ZipCode;
            address.IsDefault = dto.IsDefault;
            _repo.Update(address);
            await _repo.SaveChangesAsync();
            return new ResponseModel<ShippingAddressDto>
            {
                IsSuccess = true,
                Message = "Address updated",
                Data = MapToDto(address)
            };
        }

        public async Task<ResponseModel<bool>> SetDefaultAsync(int id, string userId)
        {
            var target = await _repo.GetByIdAsync(id);
            if (target == null || target.UserId != userId || target.IsDeleted)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "Address not found" };
            }

            var all = await _repo.GetAllAsync()
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .ToListAsync();
            foreach (var a in all)
            {
                a.IsDefault = a.Id == id;
                _repo.Update(a);
            }

            await _repo.SaveChangesAsync();
            return new ResponseModel<bool> { IsSuccess = true, Data = true };
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, string userId)
        {
            var address = await _repo.GetByIdAsync(id);
            if (address == null || address.UserId != userId || address.IsDeleted)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "Address not found" };
            }

            await _repo.DeleteAsync(id);
            return new ResponseModel<bool> { IsSuccess = true, Data = true };
        }

        private static ShippingAddressDto MapToDto(ShippingAddress a) => new()
        {
            Id = a.Id,
            Label = a.Label,
            Street = a.Street,
            City = a.City,
            State = a.State,
            Country = a.Country,
            ZipCode = a.ZipCode,
            IsDefault = a.IsDefault
        };
    }
}
