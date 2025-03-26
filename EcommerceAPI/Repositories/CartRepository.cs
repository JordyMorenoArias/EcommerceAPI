using EcommerceAPI.Data;
using EcommerceAPI.Models;
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
        /// Retrieves the cart associated with the specified user.
        /// </summary>
        /// <param name="userId">User ID to search for.</param>
        /// <returns>The user's cart or null if not found.</returns>
        public async Task<Cart?> GetCartByUserId(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        /// <summary>
        /// Creates a new cart for a specified user.
        /// </summary>
        /// <param name="userId">User ID for whom the cart is created.</param>
        /// <returns>The newly created cart.</returns>
        public async Task<Cart> CreateCart(int userId)
        {
            var existingCart = await GetCartByUserId(userId);
            if (existingCart is not null)
                return existingCart;

            var cart = new Cart { UserId = userId };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">User ID whose cart will be cleared.</param>
        public async Task ClearCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is not null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retrieves a specific item from a cart.
        /// </summary>
        /// <param name="cartId">Cart ID.</param>
        /// <param name="productId">Product ID.</param>
        /// <returns>The cart item or null if not found.</returns>
        public async Task<CartItem?> GetCartItem(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        /// <summary>
        /// Adds an item to the user's cart. If the cart does not exist, a new one is created.
        /// If the item already exists in the cart, its quantity is updated instead of adding a duplicate.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="item">Cart item to be added.</param>
        public async Task<CartItem?> AddItemToCart(int userId, CartItem item)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await GetCartByUserId(userId) ?? await CreateCart(userId);
                var existingItem = await GetCartItem(cart.Id, item.ProductId);

                if (existingItem is not null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    item.CartId = cart.Id;
                    await _context.CartItems.AddAsync(item);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return item;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        /// <summary>
        /// Updates the quantity of an item in the cart.
        /// </summary>
        /// <param name="item">Cart item to update.</param>
        /// <param name="quantity">New quantity value.</param>
        public async Task<bool> UpdateCartItemQuantity(CartItem item, int quantity)
        {
            if (item is null) 
                return false;

            if (quantity == 0)
                _context.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            return await _context.SaveChangesAsync() > 0; ;
        }

        /// <summary>
        /// Updates the price of an item in the cart.
        /// </summary>
        /// <param name="cartItemId">Cart item ID.</param>
        /// <param name="newPrice">new price of the product</param>
        public async Task<bool> UpdateCartItemPrice(int cartItemId, decimal newPrice)
        {
            if (newPrice < 0) 
                return false;

            var item = await _context.CartItems.FindAsync(cartItemId);

            if (item is null) 
                return false;

            item.UnitPrice = newPrice;
            return await _context.SaveChangesAsync() > 0; ;
        }

        /// <summary>
        /// Removes an item from the cart.
        /// </summary>
        /// <param name="cartItemId">Cart item ID to be removed.</param>
        public async Task<bool> RemoveItemFromCart(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);

            if (item is null)
                return false;

            _context.CartItems.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Retrieves all items in the cart.
        /// </summary>
        /// <param name="cartId">Cart ID.</param>
        /// <returns>List of cart items.</returns>
        public async Task<IEnumerable<CartItem>> GetCartItems(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Calculates the total price of items in the cart.
        /// </summary>
        /// <param name="cartId">Cart ID.</param>
        /// <returns>The total price of the cart.</returns>
        public async Task<decimal> GetCartTotal(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .AsNoTracking()
                .SumAsync(ci => ci.Quantity * ci.UnitPrice);
        }
    }
}
