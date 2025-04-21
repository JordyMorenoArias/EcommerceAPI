using EcommerceAPI.Models;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<bool> CartExists(int userId);
        Task<bool> ClearCart(int userId);
        Task<CartEntity> CreateCart(int userId);
        Task<CartEntity?> GetCartById(int cartId);
        Task<CartEntity?> GetCartByUserId(int userId);
        Task<decimal> GetCartTotal(int cartId);
    }
}