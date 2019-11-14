using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<PostController> _logger;

        public PostController(IBlogService blogService, ICacheClient cacheClient, ILogger<PostController> logger)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        [Route("{year:int}/{month:int}/{link}.html")]
        public IActionResult Index(int year, int month, string link)
        {
            var post = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(PostController), nameof(Index), year, month, link, !User.Identity.IsAuthenticated),
                () => _blogService.GetPost(year, month, link, !User.Identity.IsAuthenticated));

            if (post == null)
            {
                _logger.LogWarning($"Request post not exists. Year={year}, Month={month}, Link={link}.");
                return NotFound();
            }

            if (!post.IsPublic)
            {
                ViewData["Robots"] = "noindex, nofollow";

                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning(
                        $"Trying to access private post failed. Year={year}, Month={month}, Link={link}. IP={Request.HttpContext.Connection.RemoteIpAddress}, UserAgent={Request.Headers["User-Agent"]}");
                    return NotFound();
                }
            }

            post.NewAccess();
            ViewData["Canonical"] = post.FullUrlWithBase;
            ViewData["Title"] = post.Title;
            ViewData["Description"] = post.ExcerptPlain;
            ViewData["AdminView"] = HttpContext.User.Identity.IsAuthenticated;

            return View(post);
        }
    }
}