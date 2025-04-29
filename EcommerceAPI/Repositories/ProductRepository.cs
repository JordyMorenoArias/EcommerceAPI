using AutoMapper;
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
        /// <param name="context">The database context for e-commerce.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public ProductRepository(EcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the product if found; otherwise, <c>null</c>.</returns>
        public async Task<ProductEntity?> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a paginated list of products based on the given query parameters.
        /// </summary>
        /// <param name="parameters">The filtering and pagination parameters.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a paged result of products.</returns>
        public async Task<PagedResult<ProductEntity>> GetProducts(ProductQueryParameters parameters)
        {
            var query = _context.Products.AsQueryable();

            if (parameters.IsActive.HasValue)
                query = query.Where(p => p.IsActive == parameters.IsActive.Value);

            if (parameters.UserId.HasValue)
                query = query.Where(p => p.UserId == parameters.UserId.Value);

            if (parameters.Category.HasValue)
                query = query.Where(p => p.Category == parameters.Category.Value);

            if (!string.IsNullOrEmpty(parameters.SearchQuery))
            {
                var normalizedQuery = parameters.SearchQuery.ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(normalizedQuery)) ||
                    (p.Description != null && p.Description.ToLower().Contains(normalizedQuery)));
            }

            var totalItems = await query.CountAsync();

            var products = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<ProductEntity>
            {
                Items = products,
                TotalItems = totalItems,
                Page = parameters.Page,
                PageSize = parameters.PageSize,
            };
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product entity to add.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the added product.</returns>
        public async Task<ProductEntity> AddProduct(ProductEntity product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product entity with updated data.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the updated product.</returns>
        public async Task<ProductEntity> UpdateProduct(ProductEntity product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>A task representing the asynchronous operation. The task result indicates whether the deletion was successful.</returns>
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await GetProductById(id);
            if (product is null)
                return false;

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}