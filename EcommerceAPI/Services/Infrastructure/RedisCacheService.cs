using EcommerceAPI.Services.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EcommerceAPI.Services.Infrastructure
{
    /// <summary>
    /// Redis cache service implementation.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Infrastructure.Interfaces.ICacheService" />
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
        /// </summary>
        /// <param name="distributedCache">The distributed cache.</param>
        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
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
        public async Task<T?> Get<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);

            if (data is null)
                return default;

            return JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The expiration.</param>
        public async Task Set<T>(string key, T value, TimeSpan? expiration)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            var json = JsonConvert.SerializeObject(value);
            await _distributedCache.SetStringAsync(key, json, options);
        }

        /// <summary>
        /// Removes the specified key from the distributed cache.
        /// </summary>
        /// <param name="key">The key of the cached item to remove.</param>
        public Task Remove(string key)
        {
            return _distributedCache.RemoveAsync(key);
        }
    }
}