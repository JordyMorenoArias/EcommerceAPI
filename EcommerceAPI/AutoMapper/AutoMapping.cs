using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.DTOs.CartItem;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.ProductTags;
using EcommerceAPI.Models.DTOs.Tag;
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
            CreateMap<ProductEntity, SearchProductParameters>().ReverseMap();
            CreateMap<ProductEntity, ProductElasticDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    src.ProductTags.Select(pt => pt.Tag)));

            // Cart to DTO assignments and reverse
            CreateMap<CartEntity, CartDto>().ReverseMap();

            // CartItem to DTO assignments and reverse
            CreateMap<CartItemEntity, CartItemDto>().ReverseMap();

            // Address to DTO assignments and reverse
            CreateMap<AddressEntity, AddressDto>().ReverseMap();
            CreateMap<AddressEntity, AddressAddDto>().ReverseMap();
            CreateMap<AddressEntity, AddressUpdateDto>().ReverseMap();

            // Order to DTO assignments and reverse
            CreateMap<OrderEntity, OrderDto>().ReverseMap();

            // OrderDetail to DTO assignments and reverse
            CreateMap<OrderDetailEntity, OrderDetailDto>().ReverseMap();
            CreateMap<OrderDetailEntity, OrderDetailAddDto>().ReverseMap();

            // Payment to DTO assignments and reverse
            CreateMap<PaymentEntity, PaymentDto>().ReverseMap();

            // Category to DTO assignments and reverse
            CreateMap<CategoryEntity, CategoryDto>().ReverseMap();
            CreateMap<CategoryEntity, CategoryAddDto>().ReverseMap();
            CreateMap<CategoryEntity, CategoryUpdateDto>().ReverseMap();

            // Tag to DTO assignments and reverse
            CreateMap<TagEntity, TagDto>().ReverseMap();

            // ProductTag to DTO assignments and reverse
            CreateMap<ProductTagEntity, ProductTagDto>().ReverseMap();
        }
    }
}