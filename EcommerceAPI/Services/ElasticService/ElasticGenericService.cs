using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch;
using EcommerceAPI.Services.ElasticService.Interfaces;

namespace EcommerceAPI.Services.ElasticService
{
    /// <summary>
    /// Generic service for handling Elasticsearch operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="EcommerceAPI.Services.ElasticService.Interfaces.IElasticGenericService&lt;T&gt;" />
    public class ElasticGenericService<T> : IElasticGenericService<T> where T : class
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticGenericService{T}"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="indexName">Name of the index.</param>
        public ElasticGenericService(ElasticsearchClient client, string indexName)
        {
            _client = client;
            _indexName = indexName;
        }

        /// <summary>
        /// Indexes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="System.InvalidOperationException">Failed to index entity with ID {id}: {response.ElasticsearchServerError}</exception>
        public async Task Index(T item, string id)
        {
            var response = await _client.IndexAsync(item, i => i
                .Index(_indexName)
                .Id(id));

            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Failed to index entity with ID {id}: {response.ElasticsearchServerError}");
        }

        /// <summary>
        /// Indexes the many.
        /// </summary>
        /// <param name="tems">The tems.</param>
        /// <param name="idSelector">The identifier selector.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <exception cref="System.InvalidOperationException">Bulk indexing error: {string.Join("; ", errors)}</exception>
        public async Task IndexMany(IEnumerable<T> tems, Func<T, string> idSelector, int batchSize = 500)
        {
            var dtoList = tems.ToList();

            for (int i = 0; i < dtoList.Count; i += batchSize)
            {
                var batch = dtoList.Skip(i).Take(batchSize).ToList();

                var bulkRequest = new BulkRequest(_indexName)
                {
                    Operations = batch.Select(dto => new BulkIndexOperation<T>(dto)
                    {
                        Id = idSelector(dto)
                    }).Cast<IBulkOperation>().ToList()
                };

                var response = await _client.BulkAsync(bulkRequest);

                if (!response.IsValidResponse || response.Errors)
                {
                    var errors = response.ItemsWithErrors?
                        .Where(e => e.Error != null) // Ensure no null values
                        .Select(e => $"ID: {e.Id}, Error: {e.Error?.Reason}")
                        ?? Enumerable.Empty<string>(); // Handle null ItemsWithErrors

                    throw new InvalidOperationException($"Bulk indexing error: {string.Join("; ", errors)}");
                }
            }
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="System.InvalidOperationException">Failed to delete entity with ID {id}: {response.ElasticsearchServerError}</exception>
        public async Task Delete(string id)
        {
            var response = await _client.DeleteAsync<T>(id, d => d.Index(_indexName));

            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Failed to delete entity with ID {id}: {response.ElasticsearchServerError}");
        }
    }
}