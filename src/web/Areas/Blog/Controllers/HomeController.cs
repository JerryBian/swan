using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Blog.Controllers
{
    [Area(Constants.AreaBlog)]
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;

        public HomeController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            List<BlogPostView> items = await _blogService.GetAllPostsAsync();
            if (!isAuthenticated)
            {
                items = items.Where(x => x.Raw.IsPublic).ToList();
            }

            var model = items.OrderByDescending(x => x.Raw.PublishTime).ToList();
            return View(model);
        }
    }
}
