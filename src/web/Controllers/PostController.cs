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

        [HttpGet("/post/{year}/{link}")]
        public async Task<IActionResult> GetPost([FromRoute] int year, [FromRoute] string link)
        {
            var post = await _swanService.FindFirstOrDefaultAsync<SwanPost>(x => StringHelper.EqualsIgoreCase(link, x.Link) && x.PublishDate.Year == year);
            return post == null ? NotFound() : View("Detail", post);
        }

        [HttpGet("/post/{year}")]
        public async Task<IActionResult> GetPosts([FromRoute] int year)
        {
            var posts = await _swanService.FindAsync<SwanPost>(x => x.PublishDate.Year == year);
            return View("Archive", posts);
        }
    }
}
