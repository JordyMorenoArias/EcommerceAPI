
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Services.ElasticService.Interfaces
{
    /// <summary>
    /// Service for handling product-related operations in Elasticsearch.
    /// </summary>
    public interface IElasticProductService
    {
        /// <summary>
        /// Gets the suggestions products.
        /// </summary>
        /// <param name="query">The query used to search for product suggestions.</param>
        /// <returns>A collection of product name suggestions that match the query.</returns>
        Task<IEnumerable<string>> GetSuggestionsProducts(string query);

        /// <summary>
        /// Indexes multiple products in Elasticsearch in batches.
        /// </summary>
        /// <param name="products">The collection of products to index.</param>
        /// <param name="batchSize">The maximum number of products per batch (default is 500).</param>
        Task IndexManyProducts(IEnumerable<ProductEntity> products, int batchSize = 500);

        /// <summary>
        /// Indexes the product.
        /// </summary>
        /// <param name="product">The product to index in the search engine or database.</param>
        Task IndexProduct(ProductEntity product);

        /// <summary>
        /// Removes the product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to remove.</param>
        Task RemoveProduct(int productId);

        /// <summary>
        /// Searches the products.
        /// </summary>
        /// <param name="searchDto">The search parameters including filters and pagination.</param>
        /// <returns>A paged result containing the IDs of products that match the search criteria.</returns>
        Task<PagedResult<int>> SearchProducts(SearchParameters searchDto);
    }
}