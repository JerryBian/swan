using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Api.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    private readonly IGitFileService _gitFileService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IGitFileService gitFileService, ILogger<HomeController> logger)
    {
        _logger = logger;
        _gitFileService = gitFileService;
    }

    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }

    [HttpPost]
    [Route("persistent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PersistentAsync()
    {
        try
        {
            using var sr = new StreamReader(Request.Body, Encoding.UTF8);
            var message = await sr.ReadToEndAsync();
            await _gitFileService.PushAsync(message);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(PersistentAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}