using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.DTOs;
using YallaShop.Domain.Entites;

namespace YallaShop.Application.Mapping
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<SellerRequestDto, Seller>().ReverseMap();
            CreateMap<Review,ReviewRequestDto>().ReverseMap();
             CreateMap<Review, ReviewResponseDto>().ReverseMap();
        }
    }
}
