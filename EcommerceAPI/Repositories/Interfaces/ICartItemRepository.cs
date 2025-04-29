
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for cart item repository operations.
    /// </summary>
    public interface ICartItemRepository
    {
        /// <summary>
        /// Creates a new cart item.
        /// </summary>
        /// <param name="item">The cart item to create.</param>
        /// <returns>The created cart item entity.</returns>
        Task<CartItemEntity?> CreateCartItem(CartItemEntity item);

        /// <summary>
        /// Deletes a cart item by its identifier.
        /// </summary>
        /// <param name="cartItemId">The cart item identifier.</param>
        /// <returns>True if the cart item was successfully deleted, otherwise false.</returns>
        Task<bool> DeleteCartItem(int cartItemId);

        /// <summary>
        /// Retrieves a cart item by the cart ID and product ID.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>The cart item entity if found, otherwise null.</returns>
        Task<CartItemEntity?> GetCartItem(int cartId, int productId);

        /// <summary>
        /// Retrieves a cart item by its identifier.
        /// </summary>
        /// <param name="cartItemId">The cart item identifier.</param>
        /// <returns>The cart item entity if found, otherwise null.</returns>
        Task<CartItemEntity?> GetCartItemById(int cartItemId);

        /// <summary>
        /// Retrieves all items in a cart by the cart ID.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns>A collection of cart item entities.</returns>
        Task<IEnumerable<CartItemEntity>> GetCartItems(int cartId);

        /// <summary>
        /// Checks if an item exists in a cart by the cart ID and product ID.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>True if the item exists in the cart, otherwise false.</returns>
        Task<bool> ItemExists(int cartId, int productId);

        /// <summary>
        /// Updates an existing cart item.
        /// </summary>
        /// <param name="item">The cart item to update.</param>
        /// <returns>The updated cart item entity.</returns>
        Task<CartItemEntity?> UpdateCartItem(CartItemEntity item);
    }
}