using Laobian.Blog.Models;
using Laobian.Share.Blog;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

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
                () => _blogService.GetCategories(!User.Identity.IsAuthenticated),
                new BlogAssetChangeToken());
            ViewData[ViewDataConstant.Title] = "分类";
            ViewData[ViewDataConstant.Canonical] = "/category/";
            ViewData[ViewDataConstant.Description] = "所有文章以分类的形式展现";
            return View("Category", model);
        }

        [Route("/tag")]
        public IActionResult Tag()
        {
            var model = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(ArchiveController), nameof(Tag), !User.Identity.IsAuthenticated),
                () => _blogService.GetTags(!User.Identity.IsAuthenticated),
                new BlogAssetChangeToken());
            ViewData[ViewDataConstant.Title] = "标签";
            ViewData[ViewDataConstant.Canonical] = "/tag/";
            ViewData[ViewDataConstant.Description] = "所有文章以标签归类的形式展现";
            return View("Tag", model);
        }

        [Route("/archive")]
        public IActionResult Date()
        {
            var model = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(ArchiveController), nameof(Date), !User.Identity.IsAuthenticated),
                () => _blogService.GetArchives(!User.Identity.IsAuthenticated),
                new BlogAssetChangeToken());
            ViewData[ViewDataConstant.Title] = "存档";
            ViewData[ViewDataConstant.Canonical] = "/archive/";
            ViewData[ViewDataConstant.Description] = "所有文章以发表日期归类的形式展现";
            return View("Index", model);
        }
    }
}