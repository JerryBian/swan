using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IFileRepository fileRepository, ILogger<HomeController> logger)
    {
        _logger = logger;
        _fileRepository = fileRepository;
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
            await _fileRepository.SaveAsync(message);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(PersistentAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}