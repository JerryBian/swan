using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Service;

namespace Swan.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly ISwanService _swanService;

        public PostController(ISwanService swanService)
        {
            _swanService = swanService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/post/{link}.html")]
        public async Task<IActionResult> GetPost([FromRoute] string link)
        {
            var post = await _swanService.FindFirstOrDefaultAsync<SwanPost>(x => StringHelper.EqualsIgoreCase(link, x.Link));
            return post == null ? NotFound() : View("Detail", post);
        }

        [HttpGet("/post/archive")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _swanService.FindAsync<SwanPost>(Request.HttpContext);

            return !posts.Any() ? NotFound() : View("Archive", posts.GroupBy(x => x.PublishDate.Year).OrderByDescending(x => x.Key));
        }

        [HttpGet("/post/series")]
        public async Task<IActionResult> Series()
        {
            var tags = await _swanService.FindAsync<PostSeries>(Request.HttpContext);
            return View(tags.OrderBy(x => x.BlogPosts.Count));
        }

        [HttpGet("/post/tag")]
        public async Task<IActionResult> Tag()
        {
            var tags = await _swanService.FindAsync<PostTag>(Request.HttpContext);
            return View(tags.OrderBy(x => x.BlogPosts.Count));
        }
    }
}
