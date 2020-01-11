using Laobian.Blog.Models;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Laobian.Share.Extension;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.ViewComponents
{
    public class BlogInfoViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public BlogInfoViewComponent(IBlogService blogService, ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        public IViewComponentResult Invoke()
        {
            var adminView = HttpContext.User.Identity.IsAuthenticated;
            var postsCount = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(BlogInfoViewComponent), nameof(Invoke), !adminView, "POSTS"),
                () => _blogService.GetPosts(!adminView).Count);

            var model = new BlogInfo
            {
                PostsCount = postsCount.ToString(),
                //PostsAccessCount = BlogState.PostsVisitsTotal.Human(),
                //PostsAccessCountTitle = BlogState.PostsVisitsTotal.ToString(),
                Version = Global.Version,
                RunTime = Global.RuntimeString,
                RunTimeTitle = $"当前版本启动于 {Global.StartTime.ToDateAndTime()}，运行时长 {Global.Runtime}。"
            };

            return View(model);
        }
    }
}