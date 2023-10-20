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
                _logger.LogError(ex, $"Add new post item failed => {JsonHelper.Serialize(post)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/post-edit/{id}")]
        public async Task<IActionResult> EditPost([FromRoute] string id)
        {
            List<BlogPost> allPosts = await _swanService.GetBlogPostsAsync();
            BlogPost post = allPosts.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
            return post == null ? NotFound() : View("EditPost", post);
        }

        [HttpPost("/admin/post-edit")]
        public async Task<IActionResult> EditPost([FromForm] BlogPost post)
        {
            ApiResponse<object> res = new();

            try
            {
                post.Tags.Remove(string.Empty);
                post.Tags.Remove(null);
                post.IsPublic = Request.Form["isPublic"] == "on";
                post.IsDeleted = Request.Form["isDeleted"] == "on";

                await _swanService.UpdateBlogPostAsync(post);
                res.RedirectTo = post.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Edit post item failed => {JsonHelper.Serialize(post)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/tag-list")]
        public IActionResult GetAllTags()
        {
            return View("ListTag");
        }

        [HttpGet("/admin/tag-add")]
        public IActionResult AddTag()
        {
            return View("AddTag");
        }

        [HttpPut("/admin/tag-add")]
        public async Task<IActionResult> AddTag([FromForm] BlogTag tag)
        {
            ApiResponse<object> res = new();

            try
            {
                tag.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddBlogTagAsync(tag);
                res.RedirectTo = tag.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new tag item failed => {JsonHelper.Serialize(tag)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/tag-edit/{id}")]
        public async Task<IActionResult> EditTag([FromRoute] string id)
        {
            var allTags = await _swanService.GetBlogTagsAsync();
            var tag = allTags.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
            return tag == null ? NotFound() : View("EditTag", tag);
        }

        [HttpPost("/admin/tag-edit")]
        public async Task<IActionResult> EditTag([FromForm] BlogTag tag)
        {
            ApiResponse<object> res = new();

            try
            {
                tag.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.UpdateBlogTagAsync(tag);
                res.RedirectTo = tag.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Edit tag item failed => {JsonHelper.Serialize(tag)}");
            }

            return Json(res);
        }

        [HttpDelete("/admin/tag-delete/{id}")]
        public async Task<IActionResult> DeleteTag([FromRoute] string id)
        {
            ApiResponse<object> res = new();

            try
            {
                await _swanService.DeleteBlogTagAsync(id);
                res.RedirectTo = "/admin/tag-list";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Delete tag item failed => {id}");
            }

            return Json(res);
        }

        [HttpGet("/admin/series-list")]
        public IActionResult GetAllSeries()
        {
            return View("ListSeries");
        }

        [HttpGet("/admin/series-add")]
        public IActionResult AddSeries()
        {
            return View("AddSeries");
        }

        [HttpPut("/admin/series-add")]
        public async Task<IActionResult> AddSeries([FromForm] BlogSeries series)
        {
            ApiResponse<object> res = new();

            try
            {
                series.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddBlogSeriesAsync(series);
                res.RedirectTo = series.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new series item failed => {JsonHelper.Serialize(series)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/series-edit/{id}")]
        public async Task<IActionResult> EditSeries([FromRoute] string id)
        {
            var allSeries = await _swanService.GetBlogSeriesAsync();
            var tag = allSeries.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
            return tag == null ? NotFound() : View("EditSeries", tag);
        }

        [HttpPost("/admin/series-edit")]
        public async Task<IActionResult> EditSeries([FromForm] BlogSeries series)
        {
            ApiResponse<object> res = new();

            try
            {
                series.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.UpdateBlogSeriesAsync(series);
                res.RedirectTo = series.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Edit series item failed => {JsonHelper.Serialize(series)}");
            }

            return Json(res);
        }

        [HttpDelete("/admin/series-delete/{id}")]
        public async Task<IActionResult> DeleteSeries([FromRoute] string id)
        {
            ApiResponse<object> res = new();

            try
            {
                await _swanService.DeleteBlogSeriesAsync(id);
                res.RedirectTo = "/admin/series-list";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Delete series item failed => {id}");
            }

            return Json(res);
        }

        [HttpGet("/admin/read-list")]
        public IActionResult GetAllReadItems()
        {
            return View("ListRead");
        }

        [HttpGet("/admin/read-add")]
        public IActionResult AddReadItem()
        {
            return View("AddRead");
        }

        [HttpPut("/admin/read-add")]
        public async Task<IActionResult> AddRead([FromForm] ReadItem readItem)
        {
            ApiResponse<object> res = new();

            try
            {
                readItem.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddReadItemAsync(readItem);
                res.RedirectTo = readItem.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(readItem)}");
            }

            return Json(res);
        }

        [HttpGet("/admin/read-edit/{id}")]
        public async Task<IActionResult> EditReadItem([FromRoute] string id)
        {
            var allReadItems = await _swanService.GetReadItemsAsync();
            var readItem = allReadItems.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
            return readItem == null ? NotFound() : View("EditRead", readItem);
        }

        [HttpPost("/admin/read-edit")]
        public async Task<IActionResult> EditRead([FromForm] ReadItem readItem)
        {
            ApiResponse<object> res = new();

            try
            {
                readItem.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.UpdateReadItemAsync(readItem);
                res.RedirectTo = readItem.GetFullLink();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Edit read item failed => {JsonHelper.Serialize(readItem)}");
            }

            return Json(res);
        }

        [HttpDelete("/admin/read-delete/{id}")]
        public async Task<IActionResult> DeleteReadItem([FromRoute] string id)
        {
            ApiResponse<object> res = new();

            try
            {
                await _swanService.DeleteReadItemAsync(id);
                res.RedirectTo = "/admin/read-list";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Delete read item failed => {id}");
            }

            return Json(res);
        }

        [HttpGet("/admin/log")]
        public IActionResult GetLogs()
        {
            return View("Log");
        }
    }
}
