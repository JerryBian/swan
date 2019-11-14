using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class AboutController : Controller
    {
        private readonly ICacheClient _cacheClient;
        private readonly IBlogService _blogService;

        public AboutController(IBlogService blogService, ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            var html = _cacheClient.GetOrCreate(CacheKey.Build(nameof(AboutController), nameof(Index)), () => _blogService.GetAboutHtml());

            ViewData["Title"] = "关于";
            ViewData["Canonical"] = "/about/";
            ViewData["Description"] = "关于作者以及这个博客的一切";
            ViewData["AdminView"] = HttpContext.User.Identity.IsAuthenticated;
            return View(model: html);
        }
    }
}
