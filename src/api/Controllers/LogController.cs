using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
        [Route("{site}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLogs([FromRoute] string site, [FromQuery] int days)
        {
            try
            {
                var logs = new List<LaobianLog>();
                if (StringUtil.EqualsIgnoreCase(site, "all"))
                {
                    await ReadLogsAsync("blog", days, logs);
                    await ReadLogsAsync("admin", days, logs);
                    await ReadLogsAsync("api", days, logs);
                }
                else
                {
                    await ReadLogsAsync(site, days, logs);
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LogController)}({nameof(GetLogs)}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task ReadLogsAsync(string site, int days, List<LaobianLog> logs)
        {
            var logDir = Path.Combine(_option.GetDbLocation(), "log", site);
            if (!Directory.Exists(logDir))
            {
                return;
            }

            for (var i = 0; i <= days; i++)
            {
                var date = DateTime.Now.AddDays(-i);
                var logFile = Path.Combine(logDir, date.Year.ToString("D4"), date.Month.ToString("D2"), date.ToDate() + ".log");
                if (System.IO.File.Exists(logFile))
                {
                    string line;
                    using (var sr = new StreamReader(logFile))
                    {
                        while ((line = await sr.ReadLineAsync()) != null)
                        {
                            logs.Add(JsonUtil.Deserialize<LaobianLog>(line));
                        }
                    }
                }
            }
        }
    }
}