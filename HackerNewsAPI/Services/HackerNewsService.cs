using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

public class HackerNewsService : IHackerNewsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;

    public HackerNewsService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

    public async Task<IEnumerable<Story>> GetTopStoriesAsync()
    {
        if (!_cache.TryGetValue("TopStories", out List<Story> stories))
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
            var storyIds = JsonSerializer.Deserialize<List<int>>(response);

            stories = new List<Story>();
            foreach (var id in storyIds.Take(200))
            {
                var storyResponse = await client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty");
                var story = JsonSerializer.Deserialize<Story>(storyResponse);
                if (!string.IsNullOrEmpty(story.Url))
                {
                    stories.Add(story);
                }
            }

            _cache.Set("TopStories", stories, TimeSpan.FromMinutes(10));
        }
        return stories;
    }
}