using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Service;

namespace Swan.Controllers
{
    public class ReadController : Controller
    {
        private readonly IReadService _readService;
        private readonly IBlogService _blogService;
        private readonly ILogger<ReadController> _logger;

        public ReadController(IReadService readService, IBlogService blogService, ILogger<ReadController> logger)
        {
            _logger = logger;
            _readService = readService;
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            List<ReadModel> reads = await _readService.GetAllAsync(Request.HttpContext.IsAuthorized());
            return View(reads);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            List<BlogPost> posts = await _blogService.GetAllPostsAsync(true);
            return View(posts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ReadObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                _ = item.Posts.Remove(string.Empty);
                item.IsPublic = Request.Form["isPublic"] == "on";
                _ = await _readService.AddAsync(item);
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            ReadModel item = await _readService.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Posts = await _blogService.GetAllPostsAsync(true);
            ViewData["Title"] = $"编辑阅读: {item.Object.BookName}";
            return View(item);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] ReadObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                _ = item.Posts.Remove(string.Empty);
                item.IsPublic = Request.Form["isPublic"] == "on";
                _ = await _readService.UpdateAsync(item);
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
