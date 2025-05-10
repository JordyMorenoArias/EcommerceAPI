
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
        /// Searches the products.
        /// </summary>
        /// <param name="searchDto">The search parameters including filters and pagination.</param>
        /// <returns>A paged result containing the IDs of products that match the search criteria.</returns>
        Task<PagedResult<int>> SearchProducts(SearchParameters searchDto);
    }
}