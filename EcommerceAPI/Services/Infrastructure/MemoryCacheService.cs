using EcommerceAPI.Services.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Services.Infrastructure
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T?> Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

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

        public Task Remove(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
