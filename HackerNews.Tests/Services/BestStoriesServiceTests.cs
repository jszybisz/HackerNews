using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackerNews.Contracts;
using HackerNews.Services;
using Moq;
using NUnit.Framework;

namespace HackerNews.Tests.Services;

[TestFixture]
public class BestStoriesServiceTests
{
    private Mock<IHackerNewsClient> _mockHackerNewsClient;
    private BestStoriesService _bestStoriesService;

    [SetUp]
    public void Setup()
    {
        _mockHackerNewsClient = new Mock<IHackerNewsClient>();
        _bestStoriesService = new BestStoriesService(_mockHackerNewsClient.Object);
    }

    [Test]
    public async Task GetBestStories_ReturnsCorrectNumberOfItems()
    {
        // Arrange
        int numberOfItems = 3;
        var storyIds = new List<int> { 1, 2, 3, 4, 5 };
        var stories = new List<Story>
        {
            new Story { score = 100, title = "Story 1" },
            new Story { score = 200, title = "Story 2" },
            new Story { score = 50, title = "Story 3" },
            new Story { score = 150, title = "Story 4" },
            new Story { score = 120, title = "Story 5" }
        };

        _mockHackerNewsClient.Setup(client => client.GetBestStoryIdsAsync()).ReturnsAsync(storyIds);
        _mockHackerNewsClient.Setup(client => client.GetStoryDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => stories.FirstOrDefault(s => s.title.EndsWith(id.ToString())));

        // Act
        var result = await _bestStoriesService.GetBestStories(numberOfItems);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(numberOfItems, result.Count());
        Assert.AreEqual(new[] { 200, 150, 120 }, result.Select(x => x.score));
    }

    [Test]
    public async Task GetBestStories_HandlesNullStories()
    {
        // Arrange
        int numberOfItems = 3;
        var storyIds = new List<int> { 1, 2, 3, 4, 5 };
        var stories = new List<Story?>
        {
            new Story { score = 100, title = "Story 1" },
            null,
            new Story { score = 50, title = "Story 3" },
            null,
            new Story { score = 120, title = "Story 5" }
        };

        _mockHackerNewsClient.Setup(client => client.GetBestStoryIdsAsync()).ReturnsAsync(storyIds);
        _mockHackerNewsClient.Setup(client => client.GetStoryDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => stories.ElementAtOrDefault(storyIds.IndexOf(id)));

        // Act
        var result = await _bestStoriesService.GetBestStories(numberOfItems);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(numberOfItems, result.Count());
        Assert.AreEqual(new[] { 120, 100, 50 }, result.Select(x => x.score));
    }

    [Test]
    public async Task GetBestStories_EmptyStoryIds_ReturnsEmptyResult()
    {
        // Arrange
        var storyIds = new List<int>();
        _mockHackerNewsClient.Setup(client => client.GetBestStoryIdsAsync()).ReturnsAsync(storyIds);

        // Act
        var result = await _bestStoriesService.GetBestStories(5);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public void GetBestStories_ExceptionThrown_ThrowsException()
    {
        // Arrange
        _mockHackerNewsClient.Setup(client => client.GetBestStoryIdsAsync()).ThrowsAsync(new System.Exception("Something went wrong"));

        // Act & Assert
        Assert.ThrowsAsync<System.Exception>(async () => await _bestStoriesService.GetBestStories(5));
    }
}
