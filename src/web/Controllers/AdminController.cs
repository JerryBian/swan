using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Service;
using Swan.Web.Models;

namespace Swan.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISwanService _swanService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ISwanService swanService, ILogger<AdminController> logger)
        {
            _swanService = swanService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/admin/post-list")]
        public IActionResult GetAllPosts()
        {
            return View("ListPost");
        }

        [HttpGet("/admin/post-add")]
        public IActionResult AddPost()
        {
            return View("AddPost");
        }

        [HttpPut("/admin/post-add")]
        public async Task<IActionResult> AddPost([FromForm] BlogPost post)
        {
            ApiResponse<object> res = new();

            try
            {
                post.Tags.Remove(string.Empty);
                post.Tags.Remove(null);
                post.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddBlogPostAsync(post);
                res.RedirectTo = post.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(post)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/post/edit/{id}")]
        public async Task<IActionResult> EditPost(string id)
        {
            throw new NotImplementedException();
        }
    }
}
