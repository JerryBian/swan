using Laobian.Blog.Service;
using Laobian.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly LaobianBlogOption _laobianBlogOption;
        private readonly ILogger<HomeController> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public ApiController(IBlogService blogService, ILogger<HomeController> logger, IOptions<LaobianBlogOption> options, IHostApplicationLifetime appLifetime)
        {
            _blogService = blogService;
            _logger = logger;
            _appLifetime = appLifetime;
            _laobianBlogOption = options.Value;
        }

        [HttpPost]
        [Route("reload-cache")]
        public async Task<IActionResult> Reload()
        {
            if (!HttpContext.Request.Headers.ContainsKey(Constants.ApiRequestHeaderToken))
            {
                _logger.LogError(
                    $"No API token set. IP: {HttpContext.Connection.RemoteIpAddress}, User Agent: {Request.Headers[HeaderNames.UserAgent]}");
                return BadRequest("No API token set.");
            }

            if (_laobianBlogOption.HttpRequestToken != HttpContext.Request.Headers[Constants.ApiRequestHeaderToken])
            {
                _logger.LogError(
                    $"Invalid API token set: {HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}. IP: {HttpContext.Connection.RemoteIpAddress}, User Agent: {Request.Headers[HeaderNames.UserAgent]}");
                return BadRequest(
                    $"Invalid API token set: {HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}");
            }

            await _blogService.ReloadAsync();
            return Ok();
        }

        [HttpPost("shutdown")]
        public IActionResult Shutdown()
        {
            if (!HttpContext.Request.Headers.ContainsKey(Constants.ApiRequestHeaderToken))
            {
                _logger.LogError(
                    $"No API token set. IP: {HttpContext.Connection.RemoteIpAddress}, User Agent: {Request.Headers[HeaderNames.UserAgent]}");
                return BadRequest("No API token set.");
            }

            if (_laobianBlogOption.HttpRequestToken != HttpContext.Request.Headers[Constants.ApiRequestHeaderToken])
            {
                _logger.LogError(
                    $"Invalid API token set: {HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}. IP: {HttpContext.Connection.RemoteIpAddress}, User Agent: {Request.Headers[HeaderNames.UserAgent]}");
                return BadRequest(
                    $"Invalid API token set: {HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}");
            }

            _appLifetime.StopApplication();
            return Ok();
        }
    }
}
