using EcommerceAPI.Models.DTOs.Cart;
using EcommerceAPI.Models.DTOs.CartItem;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.CartItem.Interfaces
{
    /// <summary>
    /// Interface for managing cart items in a shopping cart.
    /// </summary>
    public interface ICartItemService
    {
        /// <summary>
        /// Adds an item to the user's shopping cart.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="cartItem">The cart item details to add.</param>
        /// <returns>The added cart item entity if successful, otherwise null.</returns>
        Task<CartItemDto?> AddItemToCart(int userId, CartItemAddDto cartItem);

        /// <summary>
        /// Deletes an item from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="productId">The identifier of the product to delete.</param>
        /// <returns>True if the item was successfully deleted, otherwise false.</returns>
        Task<bool> DeleteItemFromCart(int userId, int productId);

        /// <summary>
        /// Updates the quantity of an item in the user's shopping cart.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="cartItem">The cart item details with the updated quantity.</param>
        /// <returns>The updated cart item entity if successful, otherwise null.</returns>
        Task<CartItemDto?> UpdateCartItemQuantity(int userId, CartItemUpdateDto cartItem);
    }
}