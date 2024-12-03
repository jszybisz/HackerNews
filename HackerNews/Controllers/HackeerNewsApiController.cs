using HackerNews.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Controllers;

public class HackerNewsController(IBestStoriesService bestStoriesService, ILogger<HackerNewsController> logger) : ControllerBase
{
    // Get the top best story IDs
    [HttpGet("api/best-stories/{n}")]
    public async Task<IActionResult> GetBestStoriesAsync(int n)
    {
        try
        {
            var results = await bestStoriesService.GetBestStories(n);
            return Ok(results);
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, "An unexpected error occurred.");

            //NOTE: exception handling can be extended to cover other issue types like authorization error or other internal issues
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }

    }

}