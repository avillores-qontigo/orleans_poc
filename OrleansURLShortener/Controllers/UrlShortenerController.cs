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

            var grainId = "i2_dev_orleans_poc"; 

            // Create and persist a grain with the shortened ID and full URL
            var shortenerGrain = grainFactory.GetGrain<IUrlShortenerGrain>(grainId);

            await shortenerGrain.SetUrl(url);

            // Return the shortened URL for later use
            var resultBuilder = new UriBuilder(host)
            {
                Path = $"/go/{grainId}"
            };
            var pod = Environment.GetEnvironmentVariable("POD_NAME");

            return Ok($"{resultBuilder.Uri} was stored at {pod}");
        }

        [HttpGet("/go/{shortUrl}")]
        public async Task<ActionResult> GoToUrl(IGrainFactory grainFactory, [FromRoute] string shortUrl)
        {
            var shortenerGrain = grainFactory.GetGrain<IUrlShortenerGrain>(shortUrl);

            var url = await shortenerGrain.GetUrl();

            var redirectBuilder = new UriBuilder(url);

            return Redirect(redirectBuilder.Uri.ToString());
        }

        [HttpGet("/state")]
        public async Task<ActionResult<string>> State(IGrainFactory grainFactory)
        {
            var shortenerGrain = grainFactory.GetGrain<IUrlShortenerGrain>("i2_dev_orleans_poc");

            var url = await shortenerGrain.GetUrl();
            var pod = Environment.GetEnvironmentVariable("POD_NAME");

            if (string.IsNullOrEmpty(url)) 
            {
            return Ok($"No data was stored at {pod}");
            }

            var redirectBuilder = new UriBuilder(url);

            return Ok($"{redirectBuilder.Uri.ToString()} was stored at {pod}");
        }
    }
}