using Laobian.Lib;
using Laobian.Lib.Log;
using Laobian.Lib.Provider;
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
            var minLevel = LogLevel.Trace;
            if(!string.IsNullOrEmpty(level))
            {
                if(Enum.TryParse<LogLevel>(level, out var l))
                {
                    minLevel = l;
                }
            }

            var logs = _logService.ReadAllAsync(minLevel).OrderByDescending(x => x.Timestamp).ToList();
            return View(logs);
        }
    }
}
