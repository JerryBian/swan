using Laobian.Lib;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    [Authorize]
    public class BlacklistController : Controller
    {
        private readonly IBlacklistService _blacklistService;
        private readonly ILogger<BlacklistController> _logger;

        public BlacklistController(IBlacklistService blacklistService, ILogger<BlacklistController> logger)
        {
            _blacklistService = blacklistService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<BlacklistItem> model = await _blacklistService.GetAllAsync();
            return View(model);
        }

        [HttpPost("/admin/blacklist")]
        public async Task<IActionResult> Update([FromForm] BlacklistItem item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blacklistService.UdpateAsync(item);
                res.RedirectTo = "/admin/blacklist";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new blacklist item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        [HttpDelete("/admin/blacklist")]
        public async Task<IActionResult> Delete([FromQuery] string ip)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blacklistService.DeleteAsync(ip);
                res.RedirectTo = "/admin/blacklist";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Delete blacklist item failed => {ip}");
            }

            return Json(res);
        }
    }
}
