using Laobian.Lib;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Laobian.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    [Authorize]
    public class ReadController : Controller
    {
        private readonly IReadService _readService;
        private readonly IBlogService _blogService;
        private readonly ILogger<ReadController> _logger;

        public ReadController(IReadService readService, IBlogService blogService, ILogger<ReadController> logger)
        {
            _readService = readService;
            _blogService = blogService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "添加新的文章";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<BlogPostView> posts = await _blogService.GetAllPostsAsync();
            ViewData["Title"] = "添加新的阅读 - Admin";
            return View(posts.OrderByDescending(x => x.Raw.CreateTime));
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
            if (item == null)
            {
                return NotFound();
            }

            ReadItemViewModel model = new()
            {
                Posts = await _blogService.GetAllPostsAsync(),
                Item = item.Raw
            };
            ViewData["Title"] = $"编辑阅读: {item.Raw.BookName} - Admin";
            return View(model);
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
