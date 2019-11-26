using Laobian.Blog.Models;
using Laobian.Share.Blog;
using Laobian.Share.Blog.Asset;
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
                CacheKey.Build(nameof(PostController), nameof(Index), year, month, link,
                    !User.Identity.IsAuthenticated),
                () => _blogService.GetPost(year, month, link, !User.Identity.IsAuthenticated),
                new BlogAssetChangeToken());

            if (post == null)
            {
                _logger.LogWarning($"Request post not exists. Year={year}, Month={month}, Link={link}.");
                return NotFound();
            }

            if (!post.IsPublic)
            {
                ViewData[ViewDataConstant.VisibleToSearchEngine] = false;

                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning(
                        $"Trying to access private post failed. Year={year}, Month={month}, Link={link}. " +
                        $"IP={Request.HttpContext.Connection.RemoteIpAddress}, UserAgent={Request.Headers["User-Agent"]}.");
                    return NotFound();
                }
            }

            post.NewAccess();
            ViewData[ViewDataConstant.Canonical] = post.FullUrlWithBase;
            ViewData[ViewDataConstant.Title] = post.Title;
            ViewData[ViewDataConstant.Description] = post.ExcerptPlain;

            return View(post);
        }
    }
}