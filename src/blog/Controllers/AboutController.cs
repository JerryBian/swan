using System;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Laobian.Share.Extension;
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
            var viewModel = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(AboutController), nameof(Index)),
                () =>
                {
                    var posts = _blogService.GetPosts(onlyPublic: true, publishTimeDesc: true, toppingPostsFirst: false);
                    var tags = _blogService.GetTags(onlyPublic: true, publishTimeDesc: true, toppingPostsFirst: false);
                    var categories = _blogService.GetCategories(onlyPublic: true, publishTimeDesc: true, toppingPostsFirst: false);
                    var model = new AboutViewModel
                    {
                        LatestPost = posts.FirstOrDefault(),
                        PostTotalAccessCount = posts.Sum(p => p.AccessCount).ToString(),
                        PostTotalCount = posts.Count.ToString(),
                        TopPosts = posts.OrderByDescending(p => p.AccessCount).Take(Global.Config.Blog.PostsPerPage),
                        SystemAppVersion = Global.AppVersion,
                        SystemDotNetVersion = Global.RuntimeVersion,
                        SystemLastBoot = Global.StartTime.ToDateAndTime(),
                        SystemRunningInterval = Global.RunningInterval,
                        TagTotalCount = tags.Count.ToString(),
                        TopTags = tags.OrderByDescending(t => t.Posts.Count).Take(Global.Config.Blog.PostsPerPage),
                        CategoryTotalCount = categories.Count.ToString(),
                        TopCategories = categories.OrderByDescending(c => c.Posts.Count).Take(Global.Config.Blog.PostsPerPage)
                    };

                    return model;
                }, expireAfter: TimeSpan.FromHours(1));

            ViewData[ViewDataConstant.Title] = "关于";
            ViewData[ViewDataConstant.Canonical] = "/about/";
            ViewData[ViewDataConstant.Description] = "关于作者以及这个博客的一切...";
            return View(model: viewModel);
        }
    }
}