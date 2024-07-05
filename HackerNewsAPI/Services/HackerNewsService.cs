using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{

    public class HackerNewsService : IHackerNewsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public HackerNewsService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<IEnumerable<Story>> GetNewStoriesAsync()
        {
            if (!_cache.TryGetValue("TopStories", out List<Story> stories))
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");
                var storyIds = JsonSerializer.Deserialize<List<int>>(response);

                var batchSize = 10; // Adjust batch size as needed
                var batches = storyIds.Take(200)
                                      .Select((id, index) => new { id, index })
                                      .GroupBy(x => x.index / batchSize)
                                      .Select(g => g.Select(x => x.id).ToList());

                var tasks = batches.Select(async batch =>
                {
                    var batchTasks = batch.Select(async id =>
                    {
                        var storyResponse = await client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty");
                        var story = JsonSerializer.Deserialize<Story>(storyResponse);
                        return story;
                    });

                    var batchResults = await Task.WhenAll(batchTasks);
                    return batchResults.Where(story => story != null && !string.IsNullOrEmpty(story.Url));
                });

                var fetchedStories = await Task.WhenAll(tasks);
                stories = fetchedStories.SelectMany(x => x).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set("TopStories", stories, cacheOptions);
            }

            return stories;
        }

    }
}
