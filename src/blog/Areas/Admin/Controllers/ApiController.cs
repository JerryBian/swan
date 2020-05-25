using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Areas.Admin.Controllers
{
    [ApiController]
    [Authorize]
    [Route("admin/api")]
    public class ApiController : ControllerBase
    {
        [HttpPost]
        [Route("log")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Log([FromQuery]string type)
        {
            var logs = new List<LogEntry>();
            var entries = Global.InMemoryLogQueue.ToList();
            if (type == "all_logs")
            {
                logs.AddRange(entries);
            }
            else if (type == "warn_logs")
            {
                logs.AddRange(entries.Where(e => e.Level == LogLevel.Warning));
            }
            else if (type == "error_logs")
            {
                logs.AddRange(entries.Where(e => e.Level == LogLevel.Error));
            }
            else if (type == "info_logs")
            {
                logs.AddRange(entries.Where(e => e.Level == LogLevel.Information));
            }
            else
            {
                return BadRequest($"Invalid type {type}.");
            }

            logs.Reverse();
            return Ok(string.Join(Environment.NewLine, logs.Select(l =>
            {
                var log = $"[{l.When.ToDateAndTime()}]\t{l.Level}\t{l.Message}";
                if (l.Exception != null)
                {
                    log += Environment.NewLine + l.Exception;
                }

                return log;
            })));
        }
    }
}
