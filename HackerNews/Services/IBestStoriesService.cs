using HackerNews.Contracts;

namespace HackerNews.Services
{
    public interface IBestStoriesService
    {
        public Task<IEnumerable<Story>> GetBestStories(int numberOfItems);
    }
}
