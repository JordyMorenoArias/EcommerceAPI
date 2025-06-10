using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Services.ElasticService.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using EcommerceAPI.Models.DTOs.Generic;

namespace EcommerceAPI.Services.ElasticService
{
    /// <summary>
    /// Service for handling tag-related operations in Elasticsearch.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.ElasticService.Interfaces.IElasticTagService" />
    public class ElasticTagService : IElasticTagService
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName = "tags";

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticTagService"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public ElasticTagService(ElasticsearchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Searches the tags.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <see cref="PagedResult{TagDto}"/> containing the list of tags and pagination information.</returns>
        /// <exception cref="System.InvalidOperationException">Failed to search tags: {response.ElasticsearchServerError}</exception>
        public async Task<PagedResult<TagDto>> SearchTags(SearchTagParameters parameters)
        {
            int from = (parameters.Page - 1) * parameters.PageSize;

            var response = await _client.SearchAsync<TagDto>(s => s
                .Indices(_indexName)
                .From(from)
                .Size(parameters.PageSize)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(parameters.SearchTerm)
                        .Fields(new[] { "name", "description" })
                        .Type(TextQueryType.BestFields)
                        .Fuzziness("AUTO")
                        .MinimumShouldMatch("75%")
                    )           
                )
            );

            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Failed to search tags: {response.ElasticsearchServerError}");

            return new PagedResult<TagDto>
            {
                Items = response.Documents.ToList(),
                TotalItems = (int)response.Total,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }
    }
}