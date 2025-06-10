using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Tag;

namespace EcommerceAPI.Services.ElasticService.Interfaces
{
    /// <summary>
    /// Service for handling tag-related operations in Elasticsearch.
    /// </summary>
    public interface IElasticTagService
    {
        /// <summary>
        /// Searches the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <see cref="PagedResult{TagDto}"/> containing the list of tags and pagination information.</returns>
        Task<PagedResult<TagDto>> SearchTags(SearchTagParameters parameters);
    }
}