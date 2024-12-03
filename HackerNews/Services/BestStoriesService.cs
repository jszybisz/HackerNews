using HackerNews.Contracts;

namespace HackerNews.Services;

public class BestStoriesService (IHackerNewsClient hackerNewsClient) : IBestStoriesService
{
    public  async Task<IEnumerable<Story>> GetBestStories(int numberOfItems)
    {
        var storyIds = await hackerNewsClient.GetBestStoryIdsAsync();

        var storiesTask = storyIds.Select(hackerNewsClient.GetStoryDetailsAsync).ToList();
        var stories = await Task.WhenAll(storiesTask);

        var results = stories
            .Where(x => x != null)
            .OrderByDescending(x => x.score)
            .Take(numberOfItems)
            .Cast<Story>();
        return results;
    }
}