
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Services.Product.Interfaces
{
    /// <summary>
    /// Product service interface defining the operations related to product management.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Adds a new product to the system.
        /// </summary>
        /// <param name="userId">The ID of the user adding the product.</param>
        /// <param name="productAdd">The product data to be added.</param>
        /// <returns>A <see cref="ProductDto"/> representing the added product.</returns>
        Task<ProductDto> AddProduct(int userId, ProductAddDto productAdd);

        /// <summary>
        /// Gets a product by its identifier.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <returns>A <see cref="ProductDto"/> representing the product, or <c>null</c> if not found.</returns>
        Task<ProductDto?> GetProductById(int productId);

        /// <summary>
        /// Deletes a product from the system. Only authorized users can perform this operation.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the deletion.</param>
        /// <param name="role">The role of the user (e.g., Admin, Seller).</param>
        /// <param name="productId">The unique identifier of the product to be deleted.</param>
        /// <returns><c>true</c> if the product was successfully deleted; otherwise, <c>false</c>.</returns>
        Task<bool> DeleteProduct(int userId, UserRole role, int productId);

        /// <summary>
        /// Gets a list of products based on query parameters. The results can be paginated.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the products.</param>
        /// <param name="role">The role of the user (e.g., Admin, Seller, Client).</param>
        /// <param name="parameters">The query parameters to filter and paginate the product results.</param>
        /// <returns>A <see cref="PagedResult{ProductDto}"/> containing the list of products based on the query.</returns>
        Task<PagedResult<ProductDto>> GetProducts(int userId, UserRole role, QueryProductParameters parameters);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="userId">The ID of the user updating the product.</param>
        /// <param name="productUpdate">The updated product data.</param>
        /// <returns>A <see cref="ProductDto"/> representing the updated product, or <c>null</c> if not found.</returns>
        Task<ProductDto?> UpdateProduct(int userId, ProductUpdateDto productUpdate);

        /// <summary>
        /// Searches the products.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The search parameters.</param>
        /// <returns>
        /// A paginated result containing a list of <see cref="ProductDto"/> objects that match the search criteria.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Search term cannot be null or empty. - SearchTerm
        /// or
        /// Page and PageSize must be greater than 0.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Customers can only view active products.
        /// or
        /// Sellers can only view active products of other sellers.
        /// </exception>
        Task<PagedResult<ProductDto>> SearchProducts(UserRole role, SearchProductParameters searchParameters);

        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A collection of suggested product names that match the query.</returns>
        /// <exception cref="System.ArgumentException">Query cannot be null or empty. - query</exception>
        Task<IEnumerable<string>> GetSuggestionsProducts(string query);
    }
}