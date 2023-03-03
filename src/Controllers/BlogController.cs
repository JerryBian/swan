using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Option;
using Swan.Core.Service;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Swan.Controllers
{
    [Route("blog")]
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IBlogService _blogService;
        private readonly SwanOption _option;
        private readonly IBlogPostAccessService _blogPostAccessService;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger, IOptions<SwanOption> option, IBlogPostAccessService blogPostAccessService)
        {
            _logger = logger;
            _option = option.Value;
            _blogService = blogService;
            _blogPostAccessService = blogPostAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BlogPost> posts = await _blogService.GetAllPostsAsync(Request.HttpContext.IsAuthorized());
            IEnumerable<BlogPost> model = posts.Take(_option.ItemsPerPage);
            return View(model);
        }

        [HttpGet("post")]
        public async Task<IActionResult> GetPosts()
        {
            List<BlogPost> posts = await _blogService.GetAllPostsAsync(Request.HttpContext.IsAuthorized());
            return View("AllPosts", posts);
        }

        #region Posts

        [HttpGet("post/{link}.html")]
        public async Task<IActionResult> GetPost([FromRoute] string link)
        {
            BlogPost post = await _blogService.GetPostByLinkAsync(link, Request.HttpContext.IsAuthorized());
            if (post == null)
            {
                return NotFound();
            }

            _ = _blogPostAccessService.AddAsync(post.Object.Id, Request.HttpContext.GetIpAddress());
            return View("Post", post);
        }

        [Authorize]
        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetPostById([FromRoute] string id)
        {
            BlogPost post = await _blogService.GetPostAsync(id);
            return post == null ? NotFound() : View("Post", post);
        }

        [Authorize]
        [HttpGet("post/add")]
        public async Task<IActionResult> AddPost()
        {
            List<BlogTag> tags = await _blogService.GetAllTagsAsync(true);
            List<BlogSeries> series = await _blogService.GetAllSeriesAsync(true);

            ViewBag.Tags = tags;
            ViewBag.Series = series;
            return View("AddPost");
        }

        [Authorize]
        [HttpPut("post")]
        public async Task<IActionResult> AddPost([FromForm] BlogPostObject obj)
        {
            ApiResponse<object> res = new();
            try
            {
                _ = obj.Tags.Remove(string.Empty);
                obj.IsPublic = Request.Form["isPublic"] == "on";
                obj.IsTopping = Request.Form["isTopping"] == "on";
                obj.ContainsMath = Request.Form["containsMath"] == "on";
                BlogPost post = await _blogService.AddPostAsync(obj);
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
        public async Task<IActionResult> EditPost([FromRoute] string id)
        {
            BlogPost post = await _blogService.GetPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            List<BlogTag> tags = await _blogService.GetAllTagsAsync(true);
            List<BlogSeries> series = await _blogService.GetAllSeriesAsync(true);

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
                _ = obj.Tags.Remove(string.Empty);
                obj.IsPublic = Request.Form["isPublic"] == "on";
                obj.IsTopping = Request.Form["isTopping"] == "on";
                obj.ContainsMath = Request.Form["containsMath"] == "on";
                BlogPost post = await _blogService.UpdatePostAsync(obj);
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
            List<BlogTag> tags = await _blogService.GetAllTagsAsync(Request.HttpContext.IsAuthorized());
            return View("AllTags", tags);
        }

        [HttpGet("tag/{url}")]
        public async Task<IActionResult> GetTag([FromRoute] string url)
        {
            BlogTag tag = await _blogService.GetTagByUrlAsync(url, Request.HttpContext.IsAuthorized());
            return tag == null ? NotFound() : View("Tag", tag);
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
                BlogTag result = await _blogService.AddTagAsync(item);
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
            BlogTag item = await _blogService.GetTagAsync(id);
            return item == null ? NotFound() : View("EditTag", item);
        }

        [Authorize]
        [HttpPost("tag/edit")]
        public async Task<IActionResult> EditTag([FromForm] BlogTagObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                _ = await _blogService.UpdateTagAsync(item);
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
            List<BlogSeries> series = await _blogService.GetAllSeriesAsync(Request.HttpContext.IsAuthorized());
            return View("AllSeries", series);
        }

        [HttpGet("series/{url}")]
        public async Task<IActionResult> GetSeries([FromRoute] string url)
        {
            BlogSeries series = await _blogService.GetSeriesByUrlAsync(url, Request.HttpContext.IsAuthorized());
            return series == null ? NotFound() : View("series", series);
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
                _ = await _blogService.AddSeriesAsync(item);
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
            BlogSeries item = await _blogService.GetSeriesAsync(id);
            return item == null ? NotFound() : View("EditSeries", item);
        }

        [Authorize]
        [HttpPost("series/edit")]
        public async Task<IActionResult> EditSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                BlogSeries result = await _blogService.UpdateSeriesAsync(item);
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

        [Route("rss")]
        [Route("feed")]
        //[ResponseCache(CacheProfileName = Constants.CacheProfileServerShort)]
        public async Task<IActionResult> Rss()
        {
            SyndicationFeed feed = new(_option.Title, _option.Description,
                    new Uri($"{_option.BaseUrl}/blog/rss"),
                    _option.AppName, DateTimeOffset.UtcNow)
            {
                Copyright = new TextSyndicationContent(
                        $"&#x26;amp;#169; {DateTime.Now.Year} {_option.AdminUserFullName}")
            };
            feed.Authors.Add(new SyndicationPerson(_option.AdminEmail,
                _option.AdminUserFullName,
                _option.BaseUrl));
            feed.BaseUri = new Uri(_option.BaseUrl);
            feed.Language = "zh-cn";
            List<SyndicationItem> items = new();
            List<BlogPost> posts = await _blogService.GetAllPostsAsync(false);
            foreach (BlogPost post in posts)
            {
                items.Add(new SyndicationItem(post.Object.Title, post.HtmlContent,
                    new Uri($"{_option.BaseUrl}{post.GetUrl()}"),
                    $"{_option.BaseUrl}{post.GetUrl()}",
                    new DateTimeOffset(post.Object.LastUpdateTime, TimeSpan.FromHours(8))));
            }

            feed.Items = items;
            XmlWriterSettings settings = new()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = false,
                Async = true,
                Indent = true
            };

            using MemoryStream ms = new();
            using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
            {
                Rss20FeedFormatter rssFormatter = new(feed, false);
                rssFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }

            string rss = Encoding.UTF8.GetString(ms.ToArray());
            return Content(rss, "application/rss+xml", Encoding.UTF8);
        }
    }
}
