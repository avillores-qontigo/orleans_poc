using Microsoft.AspNetCore.Mvc;
using OrleansURLShortener.Grains;

namespace OrleansURLShortener.Controllers
{
    [ApiController]
    public class UrlShortenerController : ControllerBase
    {

        [HttpGet("/")]
        public ActionResult<string> Index()
        {
            return "Welcome to the URL shortener, powered by Orleans!";
        }

        [HttpGet("/shorten")]
        public async Task<ActionResult<string>> ShortenUrl(IGrainFactory grainFactory, [FromQuery] string url)
        {
            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";

            // Validate the URL query string.
            if (string.IsNullOrWhiteSpace(url) ||
                Uri.IsWellFormedUriString(url, UriKind.Absolute) is false)
            {
                return BadRequest($"""
                The URL query string is required and needs to be well formed.
                Consider, ${host}/shorten?url=https://www.microsoft.com.
                """);
            }

            var grainId = Guid.NewGuid().GetHashCode().ToString("X"); 

            // Create and persist a grain with the shortened ID and full URL
            var shortenerGrain = grainFactory.GetGrain<IUrlShortenerGrain>(grainId);

            await shortenerGrain.SetUrl(url);

            // Return the shortened URL for later use
            var resultBuilder = new UriBuilder(host)
            {
                Path = $"/go/{grainId}"
            };

            return Ok(resultBuilder.Uri);
        }

        [HttpGet("/go/{shortUrl}")]
        public async Task<ActionResult> GoToUrl(IGrainFactory grainFactory, [FromRoute] string shortUrl)
        {
            var shortenerGrain = grainFactory.GetGrain<IUrlShortenerGrain>(shortUrl);

            var url = await shortenerGrain.GetUrl();

            var redirectBuilder = new UriBuilder(url);

            return Redirect(redirectBuilder.Uri.ToString());
        }
    }
}