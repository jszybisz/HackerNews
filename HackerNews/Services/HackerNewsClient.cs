using HackerNews.Contracts;
using HackerNews.Infrastructure;
using HackerNews.Services.Cache;
using HackerNews.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNews.Services;

public class HackerNewsClient(IRestApiClient api, ICacheEngine cache , IOptions<HackerNewsApiOptions> options) : IHackerNewsClient
{
    public async Task<List<int>> GetBestStoryIdsAsync()
    {
        var cacheKey = "BestStoryIds";
        if (!cache.TryGetValue(cacheKey, out List<int>? storyIds))
        {
            storyIds = await api.GetAsync<List<int>>($"{options.Value.BaseUrl}{options.Value.BestStoriesEndpoint}");
            cache.Set(cacheKey, storyIds, options.Value.CacheTimeout);
        }
        return storyIds;
    }

    public async Task<Story?> GetStoryDetailsAsync(int id)
    {

        string cacheKey = $"Story-{id}";
        if (!cache.TryGetValue(cacheKey, out Story? story))
        {
            var sourceStory = await api.GetAsync<SourceStory>($"{options.Value.BaseUrl}{options.Value.ItemDetailsEndpoint.Replace("{id}", id.ToString())}");

            //If item doesn't exists or is deleted it should not be returned
            if (sourceStory == null || sourceStory.deleted)
            {
                return null;
            }

            story = new Story()
            {
                score = sourceStory.score,
                postedBy = sourceStory.by,
                title = sourceStory.title,
                commentCount = sourceStory.kids.Count(),
                time = DateTimeOffset.FromUnixTimeSeconds(sourceStory.time).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                uri = sourceStory.url
            };
            cache.Set(cacheKey, story, options.Value.CacheTimeout);
        }
        return story;
    }
}