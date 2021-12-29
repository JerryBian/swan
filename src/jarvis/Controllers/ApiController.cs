using Laobian.Share;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Jarvis.Controllers;

[Route("api")]
[ApiController]
[AllowAnonymous]
public class ApiController : ControllerBase
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;
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