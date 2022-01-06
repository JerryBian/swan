using System.Threading.Tasks;
using Laobian.Blog.Service;
using Laobian.Share.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IBlogService _blogService;
    private readonly ILogger<HomeController> _logger;

    public ApiController(IBlogService blogService, ILogger<HomeController> logger,
        IHostApplicationLifetime appLifetime)
    {
        _blogService = blogService;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    [HttpPost]
    [Route("cache/reload")]
    public async Task<IActionResult> Reload()
    {
        await _blogService.ReloadAsync();
        return Ok();
    }

    [HttpPost("shutdown")]
    public IActionResult Shutdown()
    {
        _logger.LogInformation("Request to shutdown blog site.");
        _appLifetime.StopApplication();
        return Ok();
    }

    [HttpPost("stat")]
    public SiteStat GetSiteStat()
    {
        return SiteStatHelper.Get();
    }
}