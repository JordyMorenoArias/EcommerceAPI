using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories
{
    public interface ICartRepository
    {
        Task AddItemToCart(int userId, CartItem item);
        Task ClearCart(int userId);
        Task<Cart> CreateCart(int userId);
        Task<Cart?> GetCartByUserId(int userId);
        Task<CartItem?> GetCartItem(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetCartItems(int cartId);
        Task<decimal> GetCartTotal(int cartId);
        Task RemoveItemFromCart(int cartItemId);
        Task UpdateCartItemPrice(int cartItemId, decimal newPrice);
        Task UpdateCartItemQuantity(CartItem item, int quantity);
    }
}