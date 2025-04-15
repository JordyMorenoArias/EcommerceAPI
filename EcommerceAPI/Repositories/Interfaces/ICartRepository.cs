using EcommerceAPI.Models;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItemEntity?> AddItemToCart(int userId, CartItemEntity item);
        Task ClearCart(int userId);
        Task<CartEntity> CreateCart(int userId);
        Task<CartEntity?> GetCartByUserId(int userId);
        Task<CartItemEntity?> GetCartItem(int cartId, int productId);
        Task<IEnumerable<CartItemEntity>> GetCartItems(int cartId);
        Task<decimal> GetCartTotal(int cartId);
        Task<bool> RemoveItemFromCart(int cartItemId);
        Task<bool> UpdateCartItemPrice(int cartItemId, decimal newPrice);
        Task<bool> UpdateCartItemQuantity(CartItemEntity item, int quantity);
    }
}