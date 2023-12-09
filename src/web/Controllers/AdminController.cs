using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Service;
using Swan.Web.Models;

namespace Swan.Web.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> AddPost([FromForm] SwanPost post)
        {
            ApiResponse<object> res = new();

            try
            {
                post.Tags.Remove(string.Empty);
                post.Tags.Remove(null);
                post.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddAsync(post);
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

        [HttpPost("/admin/post-get")]
        public async Task<IActionResult> GetPost([FromQuery]string id)
        {
            ApiResponse<SwanPost> res = new();
            try
            {
                var allPosts = await _swanService.FindAsync<SwanPost>(true);
                var post = allPosts.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
                res.Content = post;
            }
            catch(Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Get post {id} failed.");
            }

            return Json(res);
        }

        [HttpGet("/admin/post-edit/{id}")]
        public async Task<IActionResult> EditPost([FromRoute] string id)
        {
            List<SwanPost> allPosts = await _swanService.FindAsync<SwanPost>(true);
            SwanPost post = allPosts.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
            return post == null ? NotFound() : View("EditPost", post);
        }

        [HttpPost("/admin/post-edit")]
        public async Task<IActionResult> EditPost([FromForm] SwanPost post)
        {
            ApiResponse<object> res = new();

            try
            {
                post.Tags.Remove(string.Empty);
                post.Tags.Remove(null);
                post.IsPublic = Request.Form["isPublic"] == "on";
                post.IsDeleted = Request.Form["isDeleted"] == "on";

                await _swanService.UpdateAsync(post);
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
        public async Task<IActionResult> AddTag([FromForm] PostTag tag)
        {
            ApiResponse<object> res = new();

            try
            {
                tag.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddAsync(tag);
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
            var allTags = await _swanService.FindAsync<PostTag>(true);
            var tag = allTags.Find(x => StringHelper.EqualsIgoreCase(id, x.Id));
            return tag == null ? NotFound() : View("EditTag", tag);
        }

        [HttpPost("/admin/tag-edit")]
        public async Task<IActionResult> EditTag([FromForm] PostTag tag)
        {
            ApiResponse<object> res = new();

            try
            {
                tag.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.UpdateAsync(tag);
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
                await _swanService.DeleteAsync<PostTag>(id);
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
        public async Task<IActionResult> AddSeries([FromForm] PostSeries series)
        {
            ApiResponse<object> res = new();

            try
            {
                series.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddAsync(series);
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
            var series = await _swanService.FindAsync<PostSeries>(id);
            return series == null ? NotFound() : View("EditSeries", series);
        }

        [HttpPost("/admin/series-edit")]
        public async Task<IActionResult> EditSeries([FromForm] PostSeries series)
        {
            ApiResponse<object> res = new();

            try
            {
                series.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.UpdateAsync(series);
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
                await _swanService.DeleteAsync<PostSeries>(id);
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
        public async Task<IActionResult> AddRead([FromForm] SwanRead readItem)
        {
            ApiResponse<object> res = new();

            try
            {
                readItem.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.AddAsync(readItem);
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
            var readItem = await _swanService.FindAsync<SwanRead>(id);
            return readItem == null ? NotFound() : View("EditRead", readItem);
        }

        [HttpPost("/admin/read-edit")]
        public async Task<IActionResult> EditRead([FromForm] SwanRead readItem)
        {
            ApiResponse<object> res = new();

            try
            {
                readItem.IsPublic = Request.Form["isPublic"] == "on";

                await _swanService.UpdateAsync(readItem);
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
                await _swanService.DeleteAsync<SwanRead>(id);
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

        [HttpGet("/admin/stat")]
        public IActionResult GetStats()
        {
            return View("Stat");
        }

        [HttpPost("/admin/file-upload")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> UploadFile([FromForm(Name = "file")] IFormFile file)
        {
            ApiResponse<string> res = new();
            try
            {
                string fileName = StringHelper.Random();
                string ext = Path.GetExtension(file.FileName);
                await using MemoryStream ms = new();
                await file.CopyToAsync(ms);
                _ = ms.Seek(0, SeekOrigin.Begin);
                string url = await _swanService.UploadFileAsync($"file/{fileName}{ext}", ms.ToArray());
                res.Content = url;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.IsOk = false;
                _logger.LogError(ex, "File upload failed.");
            }

            return Json(res);
        }

        [HttpPost("/admin/image-upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage([FromForm(Name = "image")] IFormFile file)
        {
            try
            {
                string fileName = StringHelper.Random();
                string ext = Path.GetExtension(file.FileName);
                await using MemoryStream ms = new();
                await file.CopyToAsync(ms);
                _ = ms.Seek(0, SeekOrigin.Begin);
                string url = await _swanService.UploadFileAsync($"img/{fileName}{ext}", ms.ToArray());
                var obj = new { data = new { filePath = url } };
                return Json(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload failed.");
                var obj = new { error = 500 };
                return Json(obj);
            }
        }
    }
}
