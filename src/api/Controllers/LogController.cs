using System;
using System.Collections.Generic;
using Laobian.Api.Logger;
using Laobian.Share.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("log")]
    public class LogController : ControllerBase
    {
        private readonly IGitFileLogQueue _gitFileLogQueue;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, IGitFileLogQueue gitFileLogQueue)
        {
            _logger = logger;
            _gitFileLogQueue = gitFileLogQueue;
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
                    _gitFileLogQueue.Add(log);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LogController)}({nameof(AddLogs)}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}