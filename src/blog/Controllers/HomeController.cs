using System;
using System.Collections.Generic;
using System.Text;
using Laobian.Blog.Models;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Laobian.Share.Extension;
using Laobian.Share.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public HomeController(IBlogService blogService, ICacheClient cacheClient)
        {
            _blogService = blogService;
            _cacheClient = cacheClient;
        }

        public IActionResult Index([FromQuery] int p)
        {
            var viewModel = _cacheClient.GetOrCreate(
                $"{CacheKey.Build(nameof(HomeController), nameof(Index), p, !User.Identity.IsAuthenticated)}",
                () =>
                {
                    var posts = _blogService.GetPosts(!User.Identity.IsAuthenticated);
                    var model = new PagedPostViewModel(p, posts.Count)
                    {
                        Url = Request.Path
                    };

                    foreach (var blogPost in posts.ToPaged(Global.Config.Blog.PostsPerPage, model.CurrentPage))
                    {
                        model.Posts.Add(blogPost);
                    }

                    return model;
                });

            if (viewModel.CurrentPage > 1)
            {
                ViewData[ViewDataConstant.Title] = $"第{viewModel.CurrentPage}页";
                ViewData[ViewDataConstant.VisibleToSearchEngine] = false;
            }

            ViewData[ViewDataConstant.Canonical] = "/";
            return View(viewModel);
        }

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        public IActionResult SiteMap()
        {
            var xml = _cacheClient.GetOrCreate(
                $"{CacheKey.Build(nameof(HomeController), nameof(SiteMap))}",
                () =>
                {
                    var posts = _blogService.GetPosts();
                    var urlSet = new SiteMapUrlSet();
                    var urls = new List<SiteMapUrl>
                    {
                        new SiteMapUrl
                        {
                            Loc = Global.Config.Blog.BlogAddress,
                            ChangeFreq = "weekly",
                            LastMod = DateTime.Now.ToDate(),
                            Priority = 1.0
                        },
                        new SiteMapUrl
                        {
                            Loc = UrlHelper.Combine(Global.Config.Blog.BlogAddress, "about/"),
                            ChangeFreq = "monthly",
                            LastMod = DateTime.Now.ToDate(),
                            Priority = 0.9
                        },
                        new SiteMapUrl
                        {
                            Loc = UrlHelper.Combine(Global.Config.Blog.BlogAddress, "archive/"),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.Now.ToDate(),
                            Priority = 0.8
                        },
                        new SiteMapUrl
                        {
                            Loc = UrlHelper.Combine(Global.Config.Blog.BlogAddress, "category/"),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.Now.ToDate(),
                            Priority = 0.7
                        },
                        new SiteMapUrl
                        {
                            Loc = UrlHelper.Combine(Global.Config.Blog.BlogAddress, "tag/"),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.Now.ToDate(),
                            Priority = 0.6
                        }
                    };

                    foreach (var publishedPost in posts)
                    {
                        urls.Add(new SiteMapUrl
                        {
                            Loc = publishedPost.FullUrlWithBase,
                            ChangeFreq = "daily",
                            LastMod = publishedPost.PublishTime.ToDate(),
                            Priority = 0.5
                        });
                    }

                    urlSet.Urls = urls;
                    return SerializeHelper.ToXml(urlSet, ns: "http://www.sitemaps.org/schemas/sitemap/0.9");
                });

            return Content(xml, "text/xml", Encoding.UTF8);
        }
    }
}