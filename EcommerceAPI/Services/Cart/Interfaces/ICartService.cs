
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Cart;

namespace EcommerceAPI.Services.Cart.Interfaces
{
    /// <summary>
    /// Interface for managing the shopping cart operations.
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Clears the shopping cart for the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose cart will be cleared.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearCart(int userId);

        /// <summary>
        /// Retrieves the shopping cart for the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user whose cart will be fetched.</param>
        /// <returns>The shopping cart DTO for the user.</returns>
        Task<CartDto> GetCartByUserId(int userId);

        /// <summary>
        /// Calculates the total price of the items in the user's shopping cart.
        /// </summary>
        /// <param name="userId">The identifier of the user whose cart total will be calculated.</param>
        /// <returns>The total price of the items in the cart, or null if the cart is empty.</returns>
        Task<decimal?> GetCartTotal(int userId);
    }
}