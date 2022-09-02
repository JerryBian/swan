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
        private readonly ICacheManager _cacheManager;

        public HomeController(IBlogService blogService, ICacheManager cacheManager)
        {
            _blogService = blogService;
            _cacheManager = cacheManager;
        }

        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            List<BlogPostView> model = await _cacheManager.GetOrCreateAsync($"{Constants.AreaBlog}_{nameof(HomeController)}_{nameof(Index)}_{isAuthenticated}", async () =>
            {
                List<BlogPostView> items = await _blogService.GetAllPostsAsync();
                if (!isAuthenticated)
                {
                    items = items.Where(x => x.Raw.IsPublic).ToList();
                }

                return items.OrderByDescending(x => x.Raw.PublishTime).ToList();
            });

            return View(model);
        }
    }
}
