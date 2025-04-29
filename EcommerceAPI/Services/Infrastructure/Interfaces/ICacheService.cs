namespace EcommerceAPI.Services.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for providing caching services.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves an item from the cache.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The key associated with the cached item.</param>
        /// <returns>The cached item if found, otherwise null.</returns>
        Task<T?> Get<T>(string key);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The key of the item to remove from the cache.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Remove(string key);

        /// <summary>
        /// Adds an item to the cache with an optional expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the item to cache.</typeparam>
        /// <param name="key">The key to associate with the cached item.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The optional expiration time for the cached item. If not provided, the item will never expire.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Set<T>(string key, T value, TimeSpan? expiration = null);
    }
}