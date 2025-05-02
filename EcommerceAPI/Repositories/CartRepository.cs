using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository class for managing shopping carts in the e-commerce system.
    /// Provides methods to create, retrieve, update, and delete cart items.
    /// </summary>
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing the e-commerce data.</param>
        public CartRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Determines whether a cart exists for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// <c>true</c> if a cart exists for the user; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> CartExists(int userId)
        {
            return await _context.Carts.AnyAsync(c => c.UserId == userId);
        }

        /// <summary>
        /// Retrieves a cart by its identifier.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// <see cref="CartEntity"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<CartEntity?> GetCartById(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        /// <summary>
        /// Retrieves the cart associated with the specified user.
        /// </summary>
        /// <param name="userId">User ID to search for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// user's <see cref="CartEntity"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<CartEntity?> GetCartByUserId(int userId)
        {
            return await _context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        /// <summary>
        /// Creates a new cart for a specified user.
        /// </summary>
        /// <param name="userId">User ID for whom the cart is created.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the newly created <see cref="CartEntity"/>.
        /// </returns>
        public async Task<CartEntity> CreateCart(int userId)
        {
            var cart = new CartEntity { UserId = userId };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">User ID whose cart will be cleared.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the cart was successfully cleared; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ClearCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null || !cart.CartItems.Any()) return false;

            _context.CartItems.RemoveRange(cart.CartItems);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Calculates the total cost of all items in the specified cart.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the total price of the items in the cart.
        /// </returns>
        public async Task<decimal> GetCartTotal(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .AsNoTracking()
                .SumAsync(ci => ci.Quantity * ci.UnitPrice);
        }
    }
}