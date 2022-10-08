using Laobian.Lib;
using Laobian.Lib.Service;
using Laobian.Lib.Worker;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Blog.Controllers
{
    [Area(Constants.AreaBlog)]
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IBlogPostAccessWorker _blogPostAccessWorker;

        public PostController(IBlogService blogService, IBlogPostAccessWorker blogPostAccessWorker)
        {
            _blogService = blogService;
            _blogPostAccessWorker = blogPostAccessWorker;
        }

        [HttpGet("/blog/{year}/{month}/{link}.html")]
        [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
        public async Task<IActionResult> Index([FromRoute] int year, [FromRoute] int month, [FromRoute] string link)
        {
            Lib.Model.BlogPostView post = await _blogService.GetPostAsync(year, month, link);
            if (post == null)
            {
                return NotFound();
            }

            _blogPostAccessWorker.Add(post.Raw.Id);
            ViewData["Title"] = $"{post.Raw.Title} - 博客";
            ViewData["DatePublished"] = post.Raw.PublishTime;
            ViewData["DateModified"] = post.Raw.LastUpdateTime;
            return View(post);
        }
    }
}
