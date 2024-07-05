using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsAPI.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/GetNewStories")]
public class StoriesController : ControllerBase
{
    private readonly IHackerNewsService _hackerNewsService;

    public StoriesController(IHackerNewsService hackerNewsService)
    {
        _hackerNewsService = hackerNewsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Story>>> GetTopStories()
    {
        var stories = await _hackerNewsService.GetNewStoriesAsync();
        return Ok(stories);
    }
}