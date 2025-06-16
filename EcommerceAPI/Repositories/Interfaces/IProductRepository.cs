using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Interface for product repository operations.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Adds a new product to the repository.
        /// </summary>
        /// <param name="product">The product entity to be added.</param>
        /// <returns>The added product entity.</returns>
        Task<ProductEntity> AddProduct(ProductEntity product);

        /// <summary>
        /// Deletes a product from the repository.
        /// </summary>
        /// <param name="id">The ID of the product to be deleted.</param>
        /// <returns>True if the product was deleted, otherwise false.</returns>
        Task<bool> DeleteProduct(int id);

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The product entity if found, otherwise null.</returns>
        Task<ProductEntity?> GetProductById(int id);

        /// <summary>
        /// Retrieves a paginated list of products based on the provided query parameters.
        /// </summary>
        /// <param name="parameters">The query parameters used to filter and paginate the products.</param>
        /// <returns>A paginated result of product entities.</returns>
        Task<PagedResult<ProductEntity>> GetProducts(QueryProductParameters parameters);

        /// <summary>
        /// Gets the products by ids.
        /// </summary>
        /// <param name="ids">The collection of product IDs to retrieve.</param>
        /// <returns>A collection of <see cref="ProductEntity"/> objects matching the provided IDs.</returns>
        Task<IEnumerable<ProductEntity>> GetProductsByIds(IEnumerable<int> ids);

        /// <summary>
        /// Updates an existing product in the repository.
        /// </summary>
        /// <param name="product">The product entity with updated information.</param>
        /// <returns>The updated product entity.</returns>
        Task<ProductEntity> UpdateProduct(ProductEntity product);
    }
}