using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly ICacheClient _cacheClient;
        private readonly IBlogService _blogService;

        public ArchiveController(ICacheClient cacheClient, IBlogService blogService)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        [Route("/category")]
        public IActionResult Category()
        {
            var model = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(ArchiveController), nameof(Category), !User.Identity.IsAuthenticated),
                () => User.Identity.IsAuthenticated ? _blogService.GetCategories(false) : _blogService.GetCategories());
            ViewData["Title"] = "分类";
            ViewData["Canonical"] = "/category/";
            ViewData["Description"] = "所有文章以分类的形式展现";
            return View("Category", model);
        }

        [Route("/tag")]
        public IActionResult Tag()
        {
            var model = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(ArchiveController), nameof(Tag), !User.Identity.IsAuthenticated),
                () => User.Identity.IsAuthenticated ? _blogService.GetTags(false) : _blogService.GetTags());
            ViewData["Title"] = "标签";
            ViewData["Canonical"] = "/tag/";
            ViewData["Description"] = "所有文章以标签归类的形式展现";
            return View("Tag", model);
        }

        [Route("/archive")]
        public IActionResult Date()
        {
            var model = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(ArchiveController), nameof(Date), !User.Identity.IsAuthenticated),
                () => User.Identity.IsAuthenticated ? _blogService.GetArchives(false) : _blogService.GetArchives());
            ViewData["Title"] = "存档";
            ViewData["Canonical"] = "/archive/";
            ViewData["Description"] = "所有文章以发表日期归类的形式展现";
            return View("Index", model);
        }
    }
}