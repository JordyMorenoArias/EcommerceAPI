using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Services.ElasticService.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace EcommerceAPI.Services.ElasticProductService
{
    /// <summary>
    /// Service for handling product-related operations in Elasticsearch.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.ElasticService.Interfaces.IElasticProductService" />
    public class ElasticProductService : IElasticProductService
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName = "products";

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticProductService"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="mapper">The mapper.</param>
        public ElasticProductService(ElasticsearchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the suggestions products.
        /// </summary>
        /// <param name="query">The query used to search for product suggestions.</param>
        /// <returns>A collection of product name suggestions that match the query.</returns>
        public async Task<IEnumerable<string>> GetSuggestionsProducts(string query)
        {
            var response = await _client.SearchAsync<ProductElasticDto>(s => s
                .Indices(_indexName)
                .Size(50)
                .Query(q => q
                    .Bool(b =>
                    {
                        b.Should(sh =>
                            sh.MatchPhrasePrefix(m => m
                                .Field(f => f.Name)
                                .Query(query)
                            )
                        )
                        .Should(sh => sh
                            .MatchPhrasePrefix(m => m
                                .Field("tags.name")
                                .Query(query)
                            )
                        )
                        .MinimumShouldMatch(1);
                    })
                )
            );

            if (!response.IsValidResponse)
                return Enumerable.Empty<string>();

            return response.Documents
                .Select(p => p.Name)
                .Distinct()
                .Take(20);
        }

        /// <summary>
        /// Searches the products.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing the IDs of products that match the search criteria.</returns>
        /// <exception cref="System.InvalidOperationException">Failed to search products: {response.ElasticsearchServerError}</exception>
        public async Task<PagedResult<int>> SearchProducts(SearchProductParameters parameters)
        {

            int from = (parameters.Page - 1) * parameters.PageSize;

            var response = await _client.SearchAsync<ProductElasticDto>(s => s
                .Indices(_indexName)
                .From(from)
                .Size(parameters.PageSize)
                .Query(q => q
                    .Bool(b =>
                    {
                        b.Must(m => m
                            .MultiMatch(mm => mm
                                .Query(parameters.SearchTerm)
                                .Fields(new[] { "name", "description" })
                                .Type(TextQueryType.BestFields) // BestFields or MostFields
                                .Fuzziness("AUTO")
                                .MinimumShouldMatch("75%")
                            )
                        )
                        .Should(sh => sh
                            .MultiMatch(mm => mm
                                .Query(parameters.SearchTerm)
                                .Fields(new[] { "tags.name", "tags.description" })
                                .Type(TextQueryType.BestFields)
                                .Fuzziness("AUTO")
                                .MinimumShouldMatch("75%")
                            )
                        )
                        .Filter(f => f
                              .Term(t => t
                                .Field(f => f.IsActive)
                                .Value(parameters.IsActive ?? true)
                              )
                        )
                        .Filter(f => f
                            .Range(r => r
                                .Number(d => d
                                    .Field(f => f.Price)
                                    .Gte(parameters.MinPrice ?? 0)
                                    .Lte(parameters.MaxPrice ?? double.MaxValue)
                                )
                            )
                        );
                    })
                )
            );

            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Failed to search products: {response.ElasticsearchServerError}");

            var productsIds = response.Hits
                .Where(hit => hit.Source != null)
                .Select(hit => hit.Source!.Id);

            return new PagedResult<int>
            {
                Items = productsIds,
                TotalItems = (int)response.Total,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }
    }
}