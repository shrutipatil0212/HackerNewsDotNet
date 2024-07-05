using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsAPI.Models;

public interface IHackerNewsService
{
    Task<IEnumerable<Story>> GetNewStoriesAsync();
}