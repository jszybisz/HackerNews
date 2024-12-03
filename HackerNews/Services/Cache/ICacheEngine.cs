using HackerNews.Contracts;

namespace HackerNews.Services.Cache
{
    /// <summary>
    /// Interface that will allow to easily switch cache engine
    /// </summary>
    public interface ICacheEngine
    {
        bool TryGetValue<T>(string key, out T? story);
        void Set<T>(string key, T value, TimeSpan timeout);
    }
}
