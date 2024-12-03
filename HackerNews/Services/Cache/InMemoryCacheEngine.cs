using Microsoft.Extensions.Caching.Memory;

namespace HackerNews.Services.Cache;

/// <summary>
/// In memory cache engine is suitable when application will be run as single instance and will not be scaled horizontally
/// </summary>
public class InMemoryCacheEngine(IMemoryCache memoryCache) : ICacheEngine
{
    public bool TryGetValue<T>(string key, out T? value)
    {
        if (memoryCache.TryGetValue(key, out var cachedValue) && cachedValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    public void Set<T>(string key, T value, TimeSpan timeout) => memoryCache.Set(key, value, timeout);
   
}