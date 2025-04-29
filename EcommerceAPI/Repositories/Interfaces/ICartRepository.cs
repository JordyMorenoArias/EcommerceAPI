using EcommerceAPI.Models;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for cart repository operations.
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Checks if a cart exists for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True if the cart exists, otherwise false.</returns>
        Task<bool> CartExists(int userId);

        /// <summary>
        /// Clears the cart for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True if the cart was successfully cleared, otherwise false.</returns>
        Task<bool> ClearCart(int userId);

        /// <summary>
        /// Creates a new cart for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The created cart entity.</returns>
        Task<CartEntity> CreateCart(int userId);

        /// <summary>
        /// Retrieves a cart by its ID.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns>The cart entity if found, otherwise null.</returns>
        Task<CartEntity?> GetCartById(int cartId);

        /// <summary>
        /// Retrieves a cart by a user's ID.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The cart entity if found, otherwise null.</returns>
        Task<CartEntity?> GetCartByUserId(int userId);

        /// <summary>
        /// Calculates the total amount of the cart.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns>The total amount of the cart.</returns>
        Task<decimal> GetCartTotal(int cartId);
    }
}