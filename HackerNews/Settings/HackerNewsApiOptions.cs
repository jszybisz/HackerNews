namespace HackerNews.Settings;

public class HackerNewsApiOptions
{
    public required string BaseUrl { get; set; }
    public required string BestStoriesEndpoint { get; set; }
    public required string ItemDetailsEndpoint { get; set; }
    public required TimeSpan CacheTimeout{ get; set; }
    public required bool UseRedis { get; set; }
    public required string RedisHost { get; set; }
}