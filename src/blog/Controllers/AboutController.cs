using Laobian.Blog.Models;
using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class AboutController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public AboutController(IBlogService blogService, ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            var html = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(AboutController), nameof(Index)),
                () => _blogService.GetAboutHtml());

            ViewData[ViewDataConstant.Title] = "关于";
            ViewData[ViewDataConstant.Canonical] = "/about/";
            ViewData[ViewDataConstant.Description] = "关于作者以及这个博客的一切";
            return View(model: html);
        }
    }
}