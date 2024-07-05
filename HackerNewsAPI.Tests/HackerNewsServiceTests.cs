using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Xunit;

public class HackerNewsServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly HackerNewsService _hackerNewsService;

    public HackerNewsServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _memoryCacheMock = new Mock<IMemoryCache>();
        _memoryCache = _memoryCacheMock.Object;
        _hackerNewsService = new HackerNewsService(_httpClientFactoryMock.Object, _memoryCache);
    }

    [Fact]
    public async Task GetNewStoriesAsync_ShouldReturnStoriesFromCache_WhenCacheIsNotEmpty()
    {
        // Arrange
        var stories = new List<Story> { new Story { Id = 1, Title = "Test Story", Url = "http://test.com" } };
        object cachedStories = stories;
        _memoryCacheMock.Setup(mc => mc.TryGetValue("TopStories", out cachedStories)).Returns(true);

        // Act
        var result = await _hackerNewsService.GetNewStoriesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Story", result.First().Title);
    }

    [Fact]
    public async Task GetNewStoriesAsync_ShouldHandleEmptyStoryIdsResponse()
    {
        // Arrange
        object cachedStories;
        _memoryCacheMock.Setup(mc => mc.TryGetValue("TopStories", out cachedStories)).Returns(false);

        var cacheEntryMock = new Mock<ICacheEntry>();
        _memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("newstories.json")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new List<int>()))
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await _hackerNewsService.GetNewStoriesAsync();

        // Assert
        Assert.Empty(result);
        _memoryCacheMock.Verify(mc => mc.CreateEntry("TopStories"), Times.Once);
    }


    [Fact]
    public async Task GetNewStoriesAsync_ShouldHandleInvalidStoryIdsResponse()
    {
        // Arrange
        object cachedStories;
        _memoryCacheMock.Setup(mc => mc.TryGetValue("TopStories", out cachedStories)).Returns(false);

        var cacheEntryMock = new Mock<ICacheEntry>();
        _memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("newstories.json")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => _hackerNewsService.GetNewStoriesAsync());
    }

    [Fact]
    public async Task GetStoriesAsync_ShouldHandleInvalidStoryResponse()
    {
        // Arrange
        object cachedStories;
        _memoryCacheMock.Setup(mc => mc.TryGetValue("TopStories", out cachedStories)).Returns(false);

        var cacheEntryMock = new Mock<ICacheEntry>();
        _memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("newstories.json")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new List<int> { 1, 2, 3 }))
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("item")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => _hackerNewsService.GetNewStoriesAsync());
    }
}
