using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNews.Contracts;
using HackerNews.Infrastructure;
using HackerNews.Services;
using HackerNews.Services.Cache;
using HackerNews.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace HackerNews.Tests.Services;

[TestFixture]
public class HackerNewsClientTests
{
    private Mock<IRestApiClient> _mockApiClient;
    private Mock<ICacheEngine> _mockCache;
    private Mock<IOptions<HackerNewsApiOptions>> _mockOptions;
    private HackerNewsClient _hackerNewsClient;
    private HackerNewsApiOptions _options;

    [SetUp]
    public void Setup()
    {
        _mockApiClient = new Mock<IRestApiClient>();
        _mockCache = new Mock<ICacheEngine>();
        _mockOptions = new Mock<IOptions<HackerNewsApiOptions>>();

        _options = new HackerNewsApiOptions
        {
            BaseUrl = "https://example.com",
            BestStoriesEndpoint = "/beststories.json",
            ItemDetailsEndpoint = "/item/{id}.json",
            CacheTimeout = TimeSpan.FromMinutes(5)
        };

        _mockOptions.Setup(o => o.Value).Returns(_options);
        _hackerNewsClient = new HackerNewsClient(_mockApiClient.Object, _mockCache.Object, _mockOptions.Object);
    }

    [Test]
    public async Task GetBestStoryIdsAsync_ReturnsStoryIds_FromCache()
    {
        // Arrange
        var expectedStoryIds = new List<int> { 1, 2, 3 };
        _mockCache.Setup(c => c.TryGetValue("BestStoryIds", out expectedStoryIds)).Returns(true);

        // Act
        var result = await _hackerNewsClient.GetBestStoryIdsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedStoryIds, result);
        _mockApiClient.Verify(api => api.GetAsync<List<int>>(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetBestStoryIdsAsync_ReturnsStoryIds_FromApi_AndCachesThem()
    {
        // Arrange
        var expectedStoryIds = new List<int> { 1, 2, 3 };
        object? cacheValue = null;

        _mockCache.Setup(c => c.TryGetValue("BestStoryIds", out cacheValue)).Returns(false);
        _mockApiClient
            .Setup(api => api.GetAsync<List<int>>($"{_options.BaseUrl}{_options.BestStoriesEndpoint}"))
            .ReturnsAsync(expectedStoryIds);
        _mockCache
            .Setup(c => c.Set("BestStoryIds", expectedStoryIds, It.IsAny<TimeSpan>()))
            .Verifiable();

        // Act
        var result = await _hackerNewsClient.GetBestStoryIdsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedStoryIds, result);
        _mockCache.Verify();
    }

    [Test]
    public async Task GetStoryDetailsAsync_ReturnsStory_FromCache()
    {
        // Arrange
        var expectedStory = new Story { title = "Sample Story", score = 100 };
        _mockCache.Setup(c => c.TryGetValue("Story-1", out expectedStory)).Returns(true);

        // Act
        var result = await _hackerNewsClient.GetStoryDetailsAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedStory, result);
        _mockApiClient.Verify(api => api.GetAsync<SourceStory>(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetStoryDetailsAsync_ReturnsNull_WhenSourceStoryIsDeleted()
    {
        // Arrange
        var sourceStory = new SourceStory { deleted = true };
        object? cacheValue = null;

        _mockCache.Setup(c => c.TryGetValue("Story-1", out cacheValue)).Returns(false);
        _mockApiClient
            .Setup(api => api.GetAsync<SourceStory>($"{_options.BaseUrl}{_options.ItemDetailsEndpoint.Replace("{id}", "1")}"))
            .ReturnsAsync(sourceStory);

        // Act
        var result = await _hackerNewsClient.GetStoryDetailsAsync(1);

        // Assert
        Assert.IsNull(result);
        _mockCache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
    }
}
