using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swan.Core;
using Swan.Core.Service;

namespace Swan.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            List<Core.Model.Object.LogObject> logs = await _logService.GetAllLogsAsync();

            ViewData[Constants.ViewData.Title] = "查看日志";
            return View(logs);
        }
    }
}
