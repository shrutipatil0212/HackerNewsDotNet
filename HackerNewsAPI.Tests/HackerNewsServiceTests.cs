using HackerNewsAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace HackerNewsAPI.Tests;

public class HackerNewsServiceTests
{
    [Fact]
    public async Task GetTopStoriesAsync_ReturnsStories()
    {
        // Arrange
        var mockHttpClient = new Mock<HttpClient>();
        var mockCache = new Mock<IMemoryCache>();
        var service = new HackerNewsService(mockHttpClient.Object, mockCache.Object);

        // Act
        var result = await service.GetTopStoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any());
    }
}