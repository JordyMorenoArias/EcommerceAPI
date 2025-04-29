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

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing the e-commerce data.</param>
        public CartItemRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Determines whether a specific product exists in the cart.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the item exists in the cart; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ItemExists(int cartId, int productId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        /// <summary>
        /// Retrieves a specific cart item by cart and product identifiers.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="CartItemEntity"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<CartItemEntity?> GetCartItem(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        /// <summary>
        /// Retrieves a cart item by its unique identifier.
        /// </summary>
        /// <param name="cartItemId">The cart item identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="CartItemEntity"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<CartItemEntity?> GetCartItemById(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        /// <summary>
        /// Retrieves all items in a specific cart.
        /// </summary>
        /// <param name="cartId">The cart identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of <see cref="CartItemEntity"/> items in the cart.
        /// </returns>
        public async Task<IEnumerable<CartItemEntity>> GetCartItems(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new item to the cart.
        /// </summary>
        /// <param name="item">The cart item to add.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the newly created <see cref="CartItemEntity"/>.
        /// </returns>
        public async Task<CartItemEntity?> CreateCartItem(CartItemEntity item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Updates an existing cart item.
        /// </summary>
        /// <param name="item">The cart item with updated data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the updated <see cref="CartItemEntity"/>.
        /// </returns>
        public async Task<CartItemEntity?> UpdateCartItem(CartItemEntity item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// Deletes a cart item by its identifier.
        /// </summary>
        /// <param name="cartItemId">The cart item identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if the item was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DeleteCartItem(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item is null) return false;

            _context.CartItems.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}