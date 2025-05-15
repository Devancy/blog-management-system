using BlogManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BlogManagementSystem.Application.Services;

public class CachedAppSettingService(IAppSettingService decorated, IMemoryCache cache) : IAppSettingService
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(5);
    private const string CacheKeyPrefix = "AppSetting:";

    /// <summary>
    /// Retrieves a setting value from cache or delegates to the repository if not in cache
    /// </summary>
    public async Task<T> GetSettingAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"{CacheKeyPrefix}{key}";
        if (cache.TryGetValue(cacheKey, out T cachedValue))
        {
            return cachedValue;
        }
        
        var result = await decorated.GetSettingAsync(key, defaultValue, cancellationToken);
        if (result != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheDuration)
                .SetPriority(CacheItemPriority.Normal);
                
            cache.Set(cacheKey, result, cacheOptions);
        }
        
        return result;
    }

    /// <summary>
    /// Sets a setting value in the database and updates the cache
    /// </summary>
    public async Task SetSettingAsync<T>(string key, T value, string? description = null, CancellationToken cancellationToken = default)
    {
        await decorated.SetSettingAsync(key, value, description, cancellationToken);
        
        string cacheKey = $"{CacheKeyPrefix}{key}";
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(_cacheDuration)
            .SetPriority(CacheItemPriority.Normal);
            
        cache.Set(cacheKey, value, cacheOptions);
    }
}
