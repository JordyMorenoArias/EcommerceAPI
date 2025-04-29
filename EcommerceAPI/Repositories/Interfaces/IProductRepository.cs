using EcommerceAPI.Constants;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
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
        Task<PagedResult<ProductEntity>> GetProducts(ProductQueryParameters parameters);

        /// <summary>
        /// Updates an existing product in the repository.
        /// </summary>
        /// <param name="product">The product entity with updated information.</param>
        /// <returns>The updated product entity.</returns>
        Task<ProductEntity> UpdateProduct(ProductEntity product);
    }
}