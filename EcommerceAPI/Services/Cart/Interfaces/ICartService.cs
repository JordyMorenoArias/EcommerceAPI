
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Cart;

namespace EcommerceAPI.Services.Cart.Interfaces
{
    public interface ICartService
    {
        Task ClearCart(int userId);
        Task<CartDto> GetCartByUserId(int userId);
        Task<decimal?> GetCartTotal(int userId);
    }
}