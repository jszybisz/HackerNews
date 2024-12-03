using System.Text.Json;
using StackExchange.Redis;

namespace HackerNews.Services.Cache
{
    /// <summary>
    /// Redis cache engine is suitable when application will be run in cluster and will be scaled horizontally
    /// </summary>

    public class RedisCacheEngine : ICacheEngine
    {
        private readonly IDatabase _redisDatabase;

        public RedisCacheEngine(IConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase();
        }

        public bool TryGetValue<T>(string key, out T? value)
        {
            var cachedValue = _redisDatabase.StringGet(key);

            if (!cachedValue.IsNullOrEmpty)
            {
                value = JsonSerializer.Deserialize<T>(cachedValue!);
                return true;
            }

            value = default;
            return false;
        }

        public void Set<T>(string key, T value, TimeSpan timeout)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            _redisDatabase.StringSet(key, serializedValue, timeout);
        }
    }
}