using System;
using System.Collections.Generic;
using System.IO;
using Laobian.Share;
using Laobian.Share.Logger;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("log")]
    public class LogController : ControllerBase
    {
        private readonly ILaobianLogQueue _laobianLogQueue;
        private readonly ILogger<LogController> _logger;
        private readonly ApiOption _option;

        public LogController(ILogger<LogController> logger, ILaobianLogQueue laobianLogQueue,
            IOptions<ApiOption> config)
        {
            _logger = logger;
            _option = config.Value;
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
        [Route("{loggerName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLogs([FromRoute] LaobianSite site, [FromQuery] LogLevel level,
            [FromQuery] DateTime date)
        {
            try
            {
                var logDir = Path.Combine(_option.AssetLocation, "log");
                Directory.CreateDirectory(logDir);

                logDir = Path.Combine(logDir, site.ToString().ToLowerInvariant());
                if (!Directory.Exists(logDir))
                {
                    return Ok(string.Empty);
                }

                var logFile = Path.Combine(logDir, date.ToString("yyyy-MM-dd"));
                if (!System.IO.File.Exists(logFile))
                {
                    return Ok(string.Empty);
                }

                return Ok(JsonUtil.Serialize(System.IO.File.ReadAllText(logFile)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LogController)}({nameof(GetLogs)}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}