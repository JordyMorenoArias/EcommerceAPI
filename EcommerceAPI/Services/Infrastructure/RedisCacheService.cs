using EcommerceAPI.Services.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EcommerceAPI.Services.Infrastructure
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> Get<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);

            if (data is null)
                return default;

            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task Set<T>(string key, T value, TimeSpan? expiration)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            var json = JsonConvert.SerializeObject(value);
            await _distributedCache.SetStringAsync(key, json, options);
        }

        public Task Remove(string key)
        {
            return _distributedCache.RemoveAsync(key);
        }
    }
}