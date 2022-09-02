using Laobian.Lib;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/admin/blog/post/add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("/admin/blog/post")]
        public async Task<IActionResult> Add([FromForm] BlogPost item)
        {
            ApiResponse<object> res = new();
            try
            {
                item.IsPublic = Request.Form["isPublic"] == "on";
                item.IsTopping = Request.Form["isTopping"] == "on";
                item.ContainsMath = Request.Form["containsMath"] == "on";
                var result = await _blogService.AddPostAsync(item);
                if(result == null)
                {
                    res.IsOk = false;
                    res.Message = "Failed to add new post.";
                }
                else
                {
                    res.RedirectTo = result.FullLink;
                }
                
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new post item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/blog/post/edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute]string id)
        {
            var post = await _blogService.GetPostAsync(id);
            return View(post.Raw);
        }

        [HttpPut("/admin/blog/post")]
        public async Task<IActionResult> Edit([FromForm] BlogPost item)
        {
            ApiResponse<object> res = new();
            try
            {
                item.IsPublic = Request.Form["isPublic"] == "on";
                item.IsTopping = Request.Form["isTopping"] == "on";
                item.ContainsMath = Request.Form["containsMath"] == "on";
                var result = await _blogService.UpdateAsync(item);
                if (result == null)
                {
                    res.IsOk = false;
                    res.Message = "Failed to add update post.";
                }
                else
                {
                    res.RedirectTo = result.FullLink;
                }
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Update existing post item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }
    }
}
