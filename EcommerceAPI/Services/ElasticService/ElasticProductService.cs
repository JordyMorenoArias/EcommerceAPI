using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Services.ElasticService.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;

namespace EcommerceAPI.Services.ElasticProductService
{
    /// <summary>
    /// Service for handling product-related operations in Elasticsearch.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.ElasticService.Interfaces.IElasticProductService" />
    public class ElasticProductService : IElasticProductService
    {
        private readonly ElasticsearchClient _client;
        private readonly IMapper _mapper;
        private readonly string _indexName = "products";

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticProductService"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="mapper">The mapper.</param>
        public ElasticProductService(ElasticsearchClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
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
                    .MatchPhrasePrefix(m => m
                        .Field(f => f.Name)
                        .Query(query)
                    )
                )
            );

            if (!response.IsValidResponse)
                return Enumerable.Empty<string>();

            return response.Documents.Select(p => p.Name).Distinct().Take(20);
        }

        /// <summary>
        /// Searches the products.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing the IDs of products that match the search criteria.</returns>
        /// <exception cref="System.InvalidOperationException">Failed to search products: {response.ElasticsearchServerError}</exception>
        public async Task<PagedResult<int>> SearchProducts(SearchParameters parameters)
        {

            int from = (parameters.Page - 1) * parameters.PageSize;

            var response = await _client.SearchAsync<ProductElasticDto>(s => s
                .Indices(_indexName)
                .From(from)
                .Size(parameters.PageSize)
                .Query(q =>
                    q.Bool(b =>
                    {
                        b.Must(m => m
                            .MultiMatch(mm => mm
                                .Query(parameters.SearchTerm)
                                .Fields(new[] { "name", "description" })
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

        /// <summary>
        /// Indexes the product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <exception cref="System.InvalidOperationException">Failed to index product with ID {productDto.Id}: {errorMessage}</exception>
        public async Task IndexProduct(ProductEntity product)
        {
            var productDto = _mapper.Map<ProductElasticDto>(product);

            var response = await _client.IndexAsync(productDto, idx => idx
                .Index(_indexName)
                .Id(productDto.Id)
            );

            if (!response.IsValidResponse)
            {
                var errorMessage = response.ElasticsearchServerError?.ToString() ?? "Unknown error";
                throw new InvalidOperationException($"Failed to index product with ID {productDto.Id}: {errorMessage}");
            }
        }

        /// <summary>
        /// Indexes multiple products in Elasticsearch in batches.
        /// </summary>
        /// <param name="products">The collection of products to index.</param>
        /// <param name="batchSize">The maximum number of products per batch (default is 500).</param>
        /// <exception cref="System.InvalidOperationException">Thrown when one or more batch operations fail.</exception>
        public async Task IndexManyProducts(IEnumerable<ProductEntity> products, int batchSize = 500)
        {
            var productDtos = products.Select(p => _mapper.Map<ProductElasticDto>(p)).ToList();

            for (int i = 0; i < productDtos.Count; i += batchSize)
            {
                var batch = productDtos.Skip(i).Take(batchSize).ToList();

                var bulkRequest = new BulkRequest(_indexName)
                {
                    Operations = batch.Select(dto => new BulkIndexOperation<ProductElasticDto>(dto)
                    {
                        Id = dto.Id.ToString()
                    }).Cast<IBulkOperation>().ToList()
                };

                var response = await _client.BulkAsync(bulkRequest);

                if (!response.IsValidResponse)
                {
                    var errorMessage = response.ElasticsearchServerError?.ToString() ?? "Unknown error";
                    throw new InvalidOperationException($"Bulk index operation failed at batch {i / batchSize + 1}: {errorMessage}");
                }

                if (response.Errors)
                {
                    var failedItems = response.ItemsWithErrors.Select(e => $"ID: {e.Id}, Error: {e.Error?.Reason}");
                    var errorDetails = string.Join("; ", failedItems);
                    throw new InvalidOperationException($"Some documents failed to index in batch {i / batchSize + 1}: {errorDetails}");
                }
            }
        }

        /// <summary>
        /// Removes the product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <exception cref="System.InvalidOperationException">Failed to delete product with ID {productId}: {errorMessage}</exception>
        public async Task RemoveProduct(int productId)
        {
            var response = await _client.DeleteAsync<ProductElasticDto>(productId, idx => idx
                .Index(_indexName));

            if (!response.IsValidResponse)
            {
                var errorMessage = response.ElasticsearchServerError?.ToString() ?? "Unknown error";
                throw new InvalidOperationException($"Failed to delete product with ID {productId}: {errorMessage}");
            }

        }
    }
}