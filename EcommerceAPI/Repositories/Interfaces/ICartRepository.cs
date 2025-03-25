using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem?> AddItemToCart(int userId, CartItem item);
        Task ClearCart(int userId);
        Task<Cart> CreateCart(int userId);
        Task<Cart?> GetCartByUserId(int userId);
        Task<CartItem?> GetCartItem(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetCartItems(int cartId);
        Task<decimal> GetCartTotal(int cartId);
        Task<bool> RemoveItemFromCart(int cartItemId);
        Task<bool> UpdateCartItemPrice(int cartItemId, decimal newPrice);
        Task<bool> UpdateCartItemQuantity(CartItem item, int quantity);
    }
}