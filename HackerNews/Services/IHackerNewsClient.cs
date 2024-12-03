using HackerNews.Contracts;

namespace HackerNews.Services;

public interface IHackerNewsClient
{
    Task<List<int>> GetBestStoryIdsAsync();
    Task<Story?> GetStoryDetailsAsync(int id);
}