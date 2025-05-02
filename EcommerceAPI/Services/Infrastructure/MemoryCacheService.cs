using EcommerceAPI.Services.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Services.Infrastructure
{
    /// <summary>
    /// A simple in-memory cache service implementation.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Infrastructure.Interfaces.ICacheService" />
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService"/> class.
        /// </summary>
        /// <param name="memoryCache">The memory cache.</param>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the deserialized object 
        /// of type <typeparamref name="T"/>, or <c>default</c> if the key was not found.
        /// </returns>
        public Task<T?> Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The expiration.</param>
        public Task Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            _memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public Task Remove(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
