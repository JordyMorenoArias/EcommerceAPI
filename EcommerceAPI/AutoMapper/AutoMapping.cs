using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // User to DTO assignments and reverse
            CreateMap<UserEntity, UserAuthenticatedDto>().ReverseMap();
            CreateMap<UserEntity, UserDto>().ReverseMap();
            CreateMap<UserEntity, UserGenerateTokenDto>().ReverseMap();
            CreateMap<UserEntity, UserRegisterDto>().ReverseMap();
            CreateMap<UserEntity, UserUpdateDto>().ReverseMap();

            // Product to DTO assignments and reverse
            CreateMap<ProductEntity, ProductDto>().ReverseMap();
            CreateMap<ProductEntity, ProductAddDto>().ReverseMap();
            CreateMap<ProductEntity, ProductUpdateDto>().ReverseMap();
        }
    }
}