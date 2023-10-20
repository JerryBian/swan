using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
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

        [HttpGet("/post/{year}/{link}")]
        public async Task<IActionResult> GetPost([FromRoute] int year, [FromRoute] string link)
        {
            var posts = await _swanService.GetBlogPostsAsync();
            var post = posts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(link, x.Link));
            if (post == null || post.PublishDate.Year != year)
            {
                return NotFound();
            }

            return View("Detail", post);
        }
    }
}
