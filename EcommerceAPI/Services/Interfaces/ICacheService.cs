namespace EcommerceAPI.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T?> Get<T>(string key);
        Task Remove(string key);
        Task Set<T>(string key, T value, TimeSpan? expiration = null);
    }
}