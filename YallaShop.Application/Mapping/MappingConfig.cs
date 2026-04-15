using AutoMapper;
using System.Linq;
using YallaShop.Application.DTOs;
using YallaShop.Application.DTOs.Cart;
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

            CreateMap<Product, ProductResponseDto>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.Product != null ? s.Product.Price : 0m))
                .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Product != null ? s.Product.Price * s.Quantity : 0m))
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product));
            CreateMap<Cart, CartDto>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.CartItems.Where(ci => !ci.IsDeleted).OrderBy(ci => ci.Id)))
                .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.CartItems.Where(ci => !ci.IsDeleted).Sum(ci => ci.Product != null ? ci.Product.Price * ci.Quantity : 0m)));
        }
    }
}
