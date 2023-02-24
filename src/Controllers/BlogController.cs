using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model.Object;
using Swan.Core.Model;
using Swan.Core.Service;
using Swan.Core.Extension;
using Microsoft.Extensions.Options;
using Swan.Core.Option;
using Microsoft.AspNetCore.Authorization;

namespace Swan.Controllers
{
    [Route("blog")]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IBlogService _blogService;
        private readonly SwanOption _option;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger, IOptions<SwanOption> option)
        {
            _logger = logger;
            _option = option.Value;
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _blogService.GetAllPostsAsync(Request.HttpContext.IsAuthorized());
            var model = posts.Take(_option.ItemsPerPage);
            return View(model);
        }

        [HttpGet("post")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _blogService.GetAllPostsAsync(Request.HttpContext.IsAuthorized());
            return View("AllPosts", posts);
        }

        #region Posts

        [HttpGet("post/{link}.html")]
        public async Task<IActionResult> GetPost([FromRoute]string link)
        {
            var post = await _blogService.GetPostByLinkAsync(link, Request.HttpContext.IsAuthorized());
            if(post == null)
            {
                return NotFound();
            }

            return View("Post", post);
        }

        [Authorize]
        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetPostById([FromRoute] string id)
        {
            var post = await _blogService.GetPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View("Post", post);
        }

        [Authorize]
        [HttpGet("post/add")]
        public async Task<IActionResult> AddPost()
        {
            var tags = await _blogService.GetAllTagsAsync(true);
            var series = await _blogService.GetAllSeriesAsync(true);

            ViewBag.Tags = tags;
            ViewBag.Series = series;
            return View("AddPost");
        }

        [Authorize]
        [HttpPut("post")]
        public async Task<IActionResult> AddPost([FromForm]BlogPostObject obj)
        {
            ApiResponse<object> res = new();
            try
            {
                obj.Tags.Remove(string.Empty);
                obj.IsPublic = Request.Form["isPublic"] == "on";
                obj.IsTopping = Request.Form["isTopping"] == "on";
                obj.ContainsMath = Request.Form["containsMath"] == "on";
                var post = await _blogService.AddPostAsync(obj);
                res.RedirectTo = post.GetUrl();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(obj)}");
            }

            return Json(res);
        }

        [Authorize]
        [HttpGet("post/{id}/edit")]
        public async Task<IActionResult> EditPost([FromRoute]string id)
        {
            var post = await _blogService.GetPostAsync(id);
            if(post == null)
            {
                return NotFound();
            }

            var tags = await _blogService.GetAllTagsAsync(true);
            var series = await _blogService.GetAllSeriesAsync(true);

            ViewBag.Tags = tags;
            ViewBag.Series = series;
            return View("EditPost", post);
        }

        [Authorize]
        [HttpPost("post")]
        public async Task<IActionResult> EditPost([FromForm] BlogPostObject obj)
        {
            ApiResponse<object> res = new();
            try
            {
                obj.Tags.Remove(string.Empty);
                obj.IsPublic = Request.Form["isPublic"] == "on";
                obj.IsTopping = Request.Form["isTopping"] == "on";
                obj.ContainsMath = Request.Form["containsMath"] == "on";
                var post = await _blogService.UpdatePostAsync(obj);
                res.RedirectTo = post.GetUrl();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(obj)}");
            }

            return Json(res);
        }

        #endregion

        #region Tags

        [HttpGet("tag")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _blogService.GetAllTagsAsync(Request.HttpContext.IsAuthorized());
            return View("AllTags", tags);
        }

        [HttpGet("tag/{url}")]
        public async Task<IActionResult> GetTag([FromRoute]string url)
        {
            var tag = await _blogService.GetTagByUrlAsync(url, Request.HttpContext.IsAuthorized());
            if(tag == null)
            {
                return NotFound();
            }

            return View("Tag", tag);
        }

        [Authorize]
        [Route("tag/add")]
        public IActionResult AddTag()
        {
            return View("AddTag");
        }

        [Authorize]
        [HttpPut("tag/add")]
        public async Task<IActionResult> AddTag([FromForm] BlogTagObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                var result = await _blogService.AddTagAsync(item);
                res.RedirectTo = result.GetUrl();
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
        [HttpGet("tag/{id}/edit")]
        public async Task<IActionResult> EditTag([FromRoute] string id)
        {
            var item = await _blogService.GetTagAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View("EditTag", item);
        }

        [Authorize]
        [HttpPost("tag/edit")]
        public async Task<IActionResult> EditTag([FromForm] BlogTagObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.UpdateTagAsync(item);
                res.RedirectTo = "/blog/tag";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Update read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        #endregion

        #region Series

        [HttpGet("series")]
        public async Task<IActionResult> GetSeries()
        {
            var series = await _blogService.GetAllSeriesAsync(Request.HttpContext.IsAuthorized());
            return View("AllSeries", series);
        }

        [HttpGet("series/{url}")]
        public async Task<IActionResult> GetSeries([FromRoute] string url)
        {
            var series = await _blogService.GetSeriesByUrlAsync(url, Request.HttpContext.IsAuthorized());
            if (series == null)
            {
                return NotFound();
            }

            return View("series", series);
        }

        [Authorize]
        [Route("series/add")]
        public IActionResult AddSeries()
        {
            return View("AddSeries");
        }

        [Authorize]
        [HttpPut("series/add")]
        public async Task<IActionResult> AddSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.AddSeriesAsync(item);
                res.RedirectTo = "/blog/series";
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
        [HttpGet("series/{id}/edit")]
        public async Task<IActionResult> EditSeries([FromRoute] string id)
        {
            var item = await _blogService.GetSeriesAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View("EditSeries", item);
        }

        [Authorize]
        [HttpPost("series/edit")]
        public async Task<IActionResult> EditSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                var result = await _blogService.UpdateSeriesAsync(item);
                res.RedirectTo = result.GetUrl();
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Update read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        #endregion
    }
}
