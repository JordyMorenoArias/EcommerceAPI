using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for handling product data operations.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for ecommerce.</param>
        public ProductRepository(EcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product's identifier.</param>
        public async Task<ProductEntity?> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all products from the database regardless of their active state.
        /// </summary>
        /// <returns>A paged result of all products.</returns>
        public async Task<PagedResult<ProductDto>> GetProducts(int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalItems = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => _mapper.Map<ProductDto>(p)),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves only the active products from the database.
        /// </summary>
        /// <returns>A list of active products.</returns>
        public async Task<PagedResult<ProductDto>> GetActiveProducts(int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalItems = await query.Where(p => p.IsActive).CountAsync();
            var products = await query
                .Where(p => p.IsActive)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => _mapper.Map<ProductDto>(p)),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all products for a specific user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of products for the given user.</returns>
        public async Task<PagedResult<ProductDto>> GetProductsByUserId(int userId, int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalItems = await query.Where(p => p.UserId == userId).CountAsync();
            var products = await query
                .Where(p => p.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => _mapper.Map<ProductDto>(p)),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves only the active products for a specific user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of active products for the given user.</returns>
        public async Task<PagedResult<ProductDto>> GetActiveProductsByUserId(int userId, int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalItems = await query.Where(p => p.UserId == userId && p.IsActive).CountAsync();
            var products = await query
                .Where(p => p.UserId == userId && p.IsActive)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => _mapper.Map<ProductDto>(p)),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all products belonging to a specific category.
        /// </summary>
        /// <param name="category">The category of the products.</param>
        /// <returns>A list of products in the given category.</returns>
        public async Task<PagedResult<ProductDto>> GetProductsByCategory(CategoryProduct category, int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalItems = await query.Where(p => p.Category == category).CountAsync();
            var products = await query
                .Where(p => p.Category == category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => _mapper.Map<ProductDto>(p)),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves only the active products belonging to a specific category.
        /// </summary>
        /// <param name="category">The category of the products.</param>
        /// <returns>A list of active products in the given category.</returns>
        public async Task<PagedResult<ProductDto>> GetActiveProductsByCategory(CategoryProduct category, int page, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            var totalItems = await query.Where(p => p.Category == category && p.IsActive).CountAsync();
            var products = await query
                .Where(p => p.Category == category && p.IsActive)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => _mapper.Map<ProductDto>(p)),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves products based on a search query.
        /// </summary>
        /// <returns>A list of products matching the search query.</returns>
        public async Task<PagedResult<ProductDto>> SearchProducts(string query, int page, int pageSize)
        {
            var normalizedQuery = query.ToLower();

            var nameQuery = _context.Products
                .Where(p => p.Name != null && (p.Name.StartsWith(normalizedQuery) || p.Name.ToLower().Contains($" {normalizedQuery}")) && p.IsActive)
                .Select(p => new
                {
                    Product = p,
                    Score = 100
                });

            var descriptionQuery = _context.Products
                .Where(p => p.Description != null && p.Description.ToLower().Contains(normalizedQuery) && p.IsActive)
                .Select(p => new
                {
                    Product = p,
                    Score = 50
                });

            var nameResults = await nameQuery.AsNoTracking().ToListAsync();
            var descriptionResults = await descriptionQuery.AsNoTracking().ToListAsync();

            var combinedResults = nameResults
                .Concat(descriptionResults)
                .GroupBy(x => x.Product.Id)
                .Select(g => new
                {
                    Product = g.First().Product,
                    Score = g.Max(x => x.Score)
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Product.Name)
                .Select(x => x.Product)
                .ToList();

            var totalItems = combinedResults.Count;

            var pagedProducts = combinedResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => _mapper.Map<ProductDto>(p));

            return new PagedResult<ProductDto>
            {
                Items = pagedProducts,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <returns>The product after being added to the database.</returns>
        public async Task<ProductEntity> AddProduct(ProductEntity product)
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
        public async Task<bool> UpdateProduct(ProductEntity product)
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
