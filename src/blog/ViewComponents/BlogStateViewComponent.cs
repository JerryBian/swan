using System.Linq;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.ViewComponents
{
    public class BlogStateViewComponent : ViewComponent
    {
        private readonly ICacheClient _cacheClient;
        private readonly IBlogService _blogService;

        public BlogStateViewComponent(IBlogService blogService, ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        public IViewComponentResult Invoke(bool adminView)
        {
            var posts = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(BlogStateViewComponent), nameof(Invoke), adminView, "POSTS"),
                () => _blogService.GetPosts(!adminView).Count,
                new BlogAssetChangeToken());
            var postsAccessCount = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(BlogStateViewComponent), nameof(Invoke), adminView, "POSTS_ACCESS_COUNT"),
                () => _blogService.GetPosts(!adminView).Sum(p => p.AccessCount),
                new BlogAssetChangeToken());
            ViewData["Posts"] = posts;
            ViewData["PostsAccessCount"] = postsAccessCount;
            ViewData["Version"] = MemoryStore.Version;
            ViewData["RunTime"] = MemoryStore.RunTime;
            return View();
        }
    }
}
