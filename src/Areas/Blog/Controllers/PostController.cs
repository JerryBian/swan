using Laobian.Lib;
using Laobian.Lib.Service;
using Laobian.Lib.Worker;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Areas.Blog.Controllers
{
    [Area(Constants.AreaBlog)]
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<PostController> _logger;
        private readonly IBlogPostAccessWorker _blogPostAccessWorker;

        public PostController(IBlogService blogService, IBlogPostAccessWorker blogPostAccessWorker, ILogger<PostController> logger)
        {
            _blogService = blogService;
            _blogPostAccessWorker = blogPostAccessWorker;
            _logger = logger;
        }

        [HttpGet("/blog/{year}/{month}/{link}.html")]
        [ResponseCache(CacheProfileName = Constants.CacheProfileClientShort)]
        public async Task<IActionResult> Index([FromRoute] int year, [FromRoute] int month, [FromRoute] string link)
        {
            Lib.Model.BlogPostView post = await _blogService.GetPostAsync(year, month, link);
            if (post == null)
            {
                return NotFound();
            }

            if (!post.IsPublishedNow && Request.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning($"Attempt to access non published post, IP={Request.HttpContext.Connection.RemoteIpAddress}, URL={Request.GetDisplayUrl()}");
                return NotFound();
            }

            _blogPostAccessWorker.Add(new Lib.Model.PostAccessItem(post.Raw.Id, Request.HttpContext.Connection.RemoteIpAddress.ToString()));
            ViewData["Title"] = $"{post.Raw.Title} - 博客";
            ViewData["DatePublished"] = post.Raw.PublishTime;
            ViewData["DateModified"] = post.Raw.LastUpdateTime;
            return View(post);
        }
    }
}
