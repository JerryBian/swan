using Laobian.Lib;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    public class ReadController : Controller
    {
        private readonly IReadService _readService;
        private readonly ILogger<ReadController> _logger;

        public ReadController(IReadService readService, ILogger<ReadController> logger)
        {
            _readService = readService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ReadItem item)
        {
            ApiResponse<object> res = new();
            try
            {
                item.IsPublic = Request.Form["isPublic"] == "on";
                await _readService.AddAsync(item);
                res.RedirectTo = "/read";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            ReadItemView item = await _readService.GetAsync(id);
            return item == null ? NotFound() : View(item.Raw);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] ReadItem item)
        {
            ApiResponse<object> res = new();
            try
            {
                item.IsPublic = Request.Form["isPublic"] == "on";
                await _readService.UpdateAsync(item);
                res.RedirectTo = "/read";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Update read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }
    }
}
