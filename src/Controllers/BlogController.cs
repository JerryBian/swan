using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model.Object;
using Swan.Core.Model;
using Swan.Core.Service;
using Swan.Core.Extension;
using Microsoft.Extensions.Options;
using Swan.Core.Option;

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
            var posts = await _blogService.GetAllPostsAsync();
            var model = Request.HttpContext.IsAuthorized() ? posts.Take(_option.ItemsPerPage) : posts.Where(x => x.Object.IsPublished()).Take(_option.ItemsPerPage);
            return View(model);
        }

        [HttpGet("post")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _blogService.GetAllPostsAsync();
            var model = Request.HttpContext.IsAuthorized() ? posts : posts.Where(x => x.Object.IsPublished());
            return View("AllPosts", model);
        }

        #region Posts

        [HttpGet("post/{link}.html")]
        public async Task<IActionResult> GetPost([FromRoute]string link)
        {
            var post = await _blogService.GetPostByLinkAsync(link);
            if(post == null)
            {
                return NotFound();
            }

            return View("Post", post);
        }

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

        [HttpGet("post/add")]
        public async Task<IActionResult> AddPost()
        {
            return View("AddPost");
        }

        [HttpPut("post")]
        public async Task<IActionResult> AddPost([FromForm]BlogPostObject obj)
        {
            ApiResponse<object> res = new();
            try
            {
                var post = await _blogService.CreatePostAsync(obj);
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

        [HttpGet("post/{id}/edit")]
        public async Task<IActionResult> EditPost([FromRoute]string id)
        {
            var post = await _blogService.GetPostAsync(id);
            if(post == null)
            {
                return NotFound();
            }

            return View("EditPost", post);
        }

        [HttpPost("post")]
        public async Task<IActionResult> EditPost([FromForm] BlogPostObject obj)
        {
            ApiResponse<object> res = new();
            try
            {
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
            var tags = await _blogService.GetAllTagsAsync();
            return View("AllTags", tags);
        }

        [HttpGet("tag/{url}")]
        public async Task<IActionResult> GetTag([FromRoute]string url)
        {
            var tag = await _blogService.GetTagByUrlAsync(url);
            if(tag == null)
            {
                return NotFound();
            }

            return View("Tag", tag);
        }

        [Route("tag/add")]
        public IActionResult AddTag()
        {
            return View("AddTag");
        }

        [HttpPut("tag/add")]
        public async Task<IActionResult> AddTag([FromForm] BlogTagObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.CreateTagAsync(item);
                res.RedirectTo = "/blog/tag";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

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
            var series = await _blogService.GetAllSeriesAsync();
            return View("AllSeries", series);
        }

        [HttpGet("series/{url}")]
        public async Task<IActionResult> GetSeries([FromRoute] string url)
        {
            var series = await _blogService.GetSeriesByUrlAsync(url);
            if (series == null)
            {
                return NotFound();
            }

            return View("series", series);
        }

        [Route("series/add")]
        public IActionResult AddSeries()
        {
            return View("AddSeries");
        }

        [HttpPut("series/add")]
        public async Task<IActionResult> AddSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.CreateSeriesAsync(item);
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

        [HttpPost("series/edit")]
        public async Task<IActionResult> EditSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.UpdateSeriesAsync(item);
                res.RedirectTo = "/blog/series";
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
