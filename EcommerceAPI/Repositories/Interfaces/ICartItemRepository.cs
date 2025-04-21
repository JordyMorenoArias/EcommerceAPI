
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItemEntity?> CreateCartItem(CartItemEntity item);
        Task<bool> DeleteCartItem(int cartItemId);
        Task<CartItemEntity?> GetCartItem(int cartId, int productId);
        Task<CartItemEntity?> GetCartItemById(int cartItemId);
        Task<IEnumerable<CartItemEntity>> GetCartItems(int cartId);
        Task<bool> ItemExists(int cartId, int productId);
        Task<CartItemEntity?> UpdateCartItem(CartItemEntity item);
    }
}