using EcommerceAPI.Data;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository class for managing cart items in the e-commerce system.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Repositories.Interfaces.ICartItemRepository" />
    public class CartItemRepository : ICartItemRepository
    {
        private readonly EcommerceContext _context;

        public CartItemRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Items the exists.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        public async Task<bool> ItemExists(int cartId, int productId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        /// <summary>
        /// Gets the cart item.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        public async Task<CartItemEntity?> GetCartItem(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        /// <summary>
        /// Gets the cart item by identifier.
        /// </summary>
        /// <param name="cartItemId">The cart item identifier.</param>
        /// <returns></returns>
        public async Task<CartItemEntity?> GetCartItemById(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        /// <summary>
        /// Gets the cart items.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CartItemEntity>> GetCartItems(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Creates the cart item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public async Task<CartItemEntity?> CreateCartItem(CartItemEntity item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Updates the cart item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public async Task<CartItemEntity?> UpdateCartItem(CartItemEntity item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Deletes the cart item.
        /// </summary>
        /// <param name="cartItemId">The cart item identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeleteCartItem(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item is null) return false;

            _context.CartItems.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}