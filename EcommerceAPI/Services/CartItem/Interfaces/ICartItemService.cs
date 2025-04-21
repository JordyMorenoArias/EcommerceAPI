using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.CartItem.Interfaces
{
    public interface ICartItemService
    {
        Task<CartItemEntity?> AddItemToCart(int userId, CartItemAddDto cartItem);
        Task<bool> DeleteItemFromCart(int userId, int productId);
        Task<CartItemEntity?> UpdateCartItemQuantity(int userId, CartItemUpdateDto cartItem);
    }
}