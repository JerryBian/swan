﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Controllers;

[ApiController]
[Route("log")]
public class LogController : ControllerBase
{
    private readonly IFileRepository _fileRepository;
    private readonly ILaobianLogQueue _laobianLogQueue;
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger, ILaobianLogQueue laobianLogQueue,
        IFileRepository fileRepository)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _laobianLogQueue = laobianLogQueue;
    }

    [HttpPost]
    [Route("{loggerName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddLogs([FromRoute] string loggerName, IEnumerable<LaobianLog> logs)
    {
        try
        {
            foreach (var log in logs)
            {
                log.LoggerName = loggerName;
                _laobianLogQueue.Add(log);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LogController)}({nameof(AddLogs)}) failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("{site}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLogs([FromRoute] string site, [FromQuery] int days, [FromQuery] int minLevel)
    {
        try
        {
            var logs = new List<LaobianLog>();
            if (Enum.TryParse(site, true, out LaobianSite laobianSite))
            {
                if (laobianSite == LaobianSite.All)
                {
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Admin, days, minLevel));
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Blog, days, minLevel));
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Api, days, minLevel));
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Jarvis, days, minLevel));
                }
                else
                {
                    logs.AddRange(await ReadLogsAsync(laobianSite, days, minLevel));
                }
            }

            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LogController)}({nameof(GetLogs)}) failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private async Task<List<LaobianLog>> ReadLogsAsync(LaobianSite site, int days, int minLevel)
    {
        var result = new List<LaobianLog>();
        for (var i = 0; i <= days; i++)
        {
            var date = DateTime.Now.AddDays(-i);
            var logs = await _fileRepository.GetLogsAsync(site, date);
            result.AddRange(logs.Where(x => (int) x.Level >= minLevel));
        }

        return result;
    }
}