using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swan.Lib;
using Swan.Lib.Log;
using Swan.Lib.Service;

namespace Swan.Areas.Admin.Controllers
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
                if (Enum.TryParse(level, out LogLevel l))
                {
                    minLevel = l;
                }
            }

            List<SwanLog> logs = _logService.ReadAll(minLevel).OrderByDescending(x => x.Timestamp).ToList();
            return View(logs);
        }
    }
}
