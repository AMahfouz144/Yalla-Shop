using AutoMapper;
using YallaShop.API.ViewModels.Cart;
using YallaShop.Application.DTOs.Cart;

namespace YallaShop.API.Mapping
{
    public class MappingConfigurations:Profile
    {
        public MappingConfigurations()
        {
            CreateMap<ViewModels.RegisterViewModel, Application.DTOs.RegisterDto>().ReverseMap();
            CreateMap<ViewModels.LoginViewModel, Application.DTOs.LoginDto>().ReverseMap();

            CreateMap<ViewModels.ConfirmEmailViewModel, Application.DTOs.ConfirmEmailRequest>().ReverseMap();
            CreateMap<ViewModels.ResendEmailConfirmationViewModel, Application.DTOs.ResendEmailConfirmationRequest>().ReverseMap();
            CreateMap<ViewModels.ForgotPasswordViewModel, Application.DTOs.ForgotPasswordRequest>().ReverseMap();
            CreateMap<ViewModels.ResetPasswordViewModel, Application.DTOs.ResetPasswordRequest>().ReverseMap();

            CreateMap<ViewModels.ChangePasswordViewModel, Application.DTOs.ChangePasswordRequest>().ReverseMap();
            CreateMap<ViewModels.UpdateProfileViewModel, Application.DTOs.UpdateProfileRequest>().ReverseMap();
            CreateMap<ViewModels.UpdateEmailViewModel, Application.DTOs.UpdateEmailRequest>().ReverseMap();

            CreateMap<ViewModels.ConfirmChangeEmailViewModel, Application.DTOs.ConfirmChangeEmailRequest>().ReverseMap();

            CreateMap<ViewModels.ReviewCreateViewModel, Application.DTOs.ReviewRequestDto>().ReverseMap();
            CreateMap<ViewModels.ReviewUpdateViewModel, Application.DTOs.ReviewRequestDto>().ReverseMap();
            CreateMap<ViewModels.ReviewResponseViewModel, Application.DTOs.ReviewResponseDto>().ReverseMap();

            CreateMap<ViewModels.WishlistCreateViewModel, Application.DTOs.WishlistRequestDto>().ReverseMap();
            CreateMap<ViewModels.WishlistResponseViewModel, Application.DTOs.WishlistResponseDto>().ReverseMap();

            CreateMap<Application.DTOs.ProductResponseDto, ViewModels.ProductViewModel>().ReverseMap();
            CreateMap<Application.DTOs.WishlistResponseDto, ViewModels.WishlistResponseViewModel>().ReverseMap();

            CreateMap<AddToCartRequestVm, AddToCartDto>();
            CreateMap<UpdateCartItemRequestVm, UpdateCartItemDto>();
            CreateMap<CartDto, CartResponseVm>();
            CreateMap<CartItemDto, CartItemResponseVm>();
        }
    }
}
