using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for handling product data operations.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for ecommerce.</param>
        public ProductRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all products from the database regardless of their active state.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product's identifier.</param>
        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Retrieves only the active products from the database.
        /// </summary>
        /// <returns>A list of active products.</returns>
        public async Task<IEnumerable<Product>> GetActiveProducts()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all products for a specific user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of products for the given user.</returns>
        public async Task<IEnumerable<Product>> GetProductsByUserId(int userId)
        {
            return await _context.Products
                .Where(p => p.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves only the active products for a specific user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of active products for the given user.</returns>
        public async Task<IEnumerable<Product>> GetActiveProductsByUserId(int userId)
        {
            return await _context.Products
                .Where(p => p.UserId == userId && p.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all products belonging to a specific category.
        /// </summary>
        /// <param name="category">The category of the products.</param>
        /// <returns>A list of products in the given category.</returns>
        public async Task<IEnumerable<Product>> GetProductsByCategory(CategoryProduct category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves only the active products belonging to a specific category.
        /// </summary>
        /// <param name="category">The category of the products.</param>
        /// <returns>A list of active products in the given category.</returns>
        public async Task<IEnumerable<Product>> GetActiveProductsByCategory(CategoryProduct category)
        {
            return await _context.Products
                .Where(p => p.Category == category && p.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <returns>The product after being added to the database.</returns>
        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product with updated information.</param>
        /// <returns>The updated product.</returns>
        public async Task<bool> UpdateProduct(Product product)
        {
            var existingProduct = await GetProductById(product.Id);
            if (existingProduct is null)
                return false;

            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Deletes a product from the database by its identifier.
        /// </summary>
        /// <param name="id">The product's identifier.</param>
        /// <returns>The deleted product, or null if not found.</returns>
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await GetProductById(id);

            if (product is null)
                return false;

            _context.Products.Remove(product!);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
