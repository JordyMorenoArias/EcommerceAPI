using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.DTOs.CartItem;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.DTOs.Payment;
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
        }
    }
}