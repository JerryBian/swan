using Microsoft.AspNetCore.Mvc;
using Laobian.Blog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Model;
using Laobian.Share.Cache;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Laobian.Share.Helper;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly ICacheClient _cacheClient;
        private readonly IBlogService _blogService;

        public HomeController(IBlogService blogService, ICacheClient cacheClient, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
            _cacheClient = cacheClient;
        }

        public async Task<IActionResult> Index([FromQuery] int p)
        {
            var viewModel = await _cacheClient.GetOrCreateAsync(
                $"{nameof(HomeController)}:{nameof(Index)}:{p}:{User.Identity.IsAuthenticated}",
                async () =>
                {
                    List<BlogPost> posts;
                    if (User.Identity.IsAuthenticated)
                    {
                        posts = await _blogService.GetPostsAsync(false);
                    }
                    else
                    {
                        posts = await _blogService.GetPostsAsync();
                    }

                    var model = new PagedPostViewModel(p, posts.Count)
                    {
                        Url = Request.Path
                    };

                    foreach (var blogPost in posts.ToPaged(_appConfig.Blog.PostsPerPage, model.CurrentPage))
                    {
                        model.Posts.Add(new PostViewModel(blogPost));
                    }

                    if (model.CurrentPage > 1)
                    {
                        ViewData["Title"] = $"第{model.CurrentPage}页";
                        ViewData["Robots"] = "noindex, nofollow";
                    }

                    ViewData["Canonical"] = "/";

                    return model;
                }, new BlogAssetChangeToken());


            return View(viewModel);
        }

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        public async Task<IActionResult> SiteMap()
        {
            var xml = await _cacheClient.GetOrCreateAsync(
                $"{nameof(HomeController)}:{nameof(SiteMap)}",
                async () =>
                {
                    var posts = await _blogService.GetPostsAsync();
                    var urlSet = new SiteMapUrlSet();
                    var urls = new List<SiteMapUrl>
                    {
                        new SiteMapUrl
                        {
                            Loc = AddressHelper.GetAddress(_appConfig.Blog.BlogAddress),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                            Priority = 1.0
                        },
                        new SiteMapUrl
                        {
                            Loc = AddressHelper.GetAddress(_appConfig.Blog.BlogAddress, true, "about"),
                            ChangeFreq = "monthly",
                            LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                            Priority = 0.9
                        },
                        new SiteMapUrl
                        {
                            Loc = AddressHelper.GetAddress(_appConfig.Blog.BlogAddress, true, "archive"),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                            Priority = 0.8
                        },
                        new SiteMapUrl
                        {
                            Loc = AddressHelper.GetAddress(_appConfig.Blog.BlogAddress, true, "category"),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                            Priority = 0.7
                        },
                        new SiteMapUrl
                        {
                            Loc = AddressHelper.GetAddress(_appConfig.Blog.BlogAddress, true, "tag"),
                            ChangeFreq = "weekly",
                            LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
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
                }, new BlogAssetChangeToken());

            return Content(xml, "text/xml", Encoding.UTF8);
        }
    }
}
