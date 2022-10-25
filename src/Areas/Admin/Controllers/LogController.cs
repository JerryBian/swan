using Laobian.Lib;
using Laobian.Lib.Log;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    [Authorize]
    public class LogController : Controller
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        public IActionResult Index([FromQuery] string level)
        {
            LogLevel minLevel = LogLevel.Trace;
            if (!string.IsNullOrEmpty(level))
            {
                if (Enum.TryParse<LogLevel>(level, out LogLevel l))
                {
                    minLevel = l;
                }
            }

            List<LaobianLog> logs = _logService.ReadAll(minLevel).OrderByDescending(x => x.Timestamp).ToList();
            return View(logs);
        }
    }
}
