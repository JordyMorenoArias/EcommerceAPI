

namespace EcommerceAPI.Services.ElasticService.Interfaces
{
    public interface IElasticGenericService<T> where T : class
    {
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="System.InvalidOperationException">Failed to delete entity with ID {id}: {response.ElasticsearchServerError}</exception>
        Task Delete(string id);

        /// <summary>
        /// Indexes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="System.InvalidOperationException">Failed to index entity with ID {id}: {response.ElasticsearchServerError}</exception>
        Task Index(T item, string id);

        /// <summary>
        /// Indexes the many.
        /// </summary>
        /// <param name="tems">The tems.</param>
        /// <param name="idSelector">The identifier selector.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <exception cref="System.InvalidOperationException">Bulk indexing error: {string.Join("; ", errors)}</exception>
        Task IndexMany(IEnumerable<T> tems, Func<T, string> idSelector, int batchSize = 500);
    }
}