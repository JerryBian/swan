using Microsoft.AspNetCore.Mvc;
using Laobian.Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Laobian.Share.Helper;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public HomeController(IBlogService blogService, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        public IActionResult Index([FromQuery] int p)
        {
            const int pageSize = 8;
            var publishedPosts = _blogService.GetPosts().Where(_ => _.IsPublic).OrderByDescending(_ => _.CreationTimeUtc).ToList();
            var pagination = new Pagination(p, (int)Math.Ceiling(publishedPosts.Count() / (double)pageSize));
            var posts = publishedPosts.ToPaged(pageSize, pagination.CurrentPage);
            var categories = _blogService.GetCategories();
            var tags = _blogService.GetTags();
            var postViewModels = new List<PostViewModel>();
            foreach (var blogPost in posts)
            {
                var postViewModel = new PostViewModel(blogPost);
                foreach (var blogPostCategoryName in blogPost.CategoryNames)
                {
                    var cat = categories.FirstOrDefault(_ =>
                        string.Equals(_.Name, blogPostCategoryName, StringComparison.OrdinalIgnoreCase));
                    if (cat != null)
                    {
                        postViewModel.Categories.Add(cat);
                    }
                }

                foreach (var blogPostTagName in blogPost.TagNames)
                {
                    var tag = tags.FirstOrDefault(_ =>
                        string.Equals(_.Name, blogPostTagName, StringComparison.OrdinalIgnoreCase));
                    if (tag != null)
                    {
                        postViewModel.Tags.Add(tag);
                    }
                }

                postViewModels.Add(postViewModel);
            }

            if (pagination.CurrentPage > 1)
            {
                ViewData["Title"] = $"第{pagination.CurrentPage}页";
                ViewData["Robots"] = "noindex, nofollow";
            }

            ViewData["Canonical"] = "/";
            return View(new PagedPostViewModel { Pagination = pagination, Posts = postViewModels, Url = Request.Path });
        }

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        public IActionResult SiteMap()
        {
            var publishedPosts = _blogService.GetPosts().Where(_ => _.IsPublic)
                .OrderByDescending(_ => _.CreationTimeUtc).ToList();
            var urlSet = new SiteMapUrlSet();
            var urls = new List<SiteMapUrl>
            {
                new SiteMapUrl
                {
                    Loc = AddressHelper.GetAddress(_appConfig.BlogAddress),
                    ChangeFreq = "weekly",
                    LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                    Priority = 1.0
                },
                new SiteMapUrl
                {
                    Loc = AddressHelper.GetAddress(_appConfig.BlogAddress, true, "about"),
                    ChangeFreq = "monthly",
                    LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                    Priority = 0.9
                },
                new SiteMapUrl
                {
                    Loc = AddressHelper.GetAddress(_appConfig.BlogAddress, true, "archive"),
                    ChangeFreq = "weekly",
                    LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                    Priority = 0.8
                },
                new SiteMapUrl
                {
                    Loc = AddressHelper.GetAddress(_appConfig.BlogAddress, true, "category"),
                    ChangeFreq = "weekly",
                    LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                    Priority = 0.7
                },
                new SiteMapUrl
                {
                    Loc = AddressHelper.GetAddress(_appConfig.BlogAddress, true, "tag"),
                    ChangeFreq = "weekly",
                    LastMod = DateTime.UtcNow.ToChinaTime().ToDate(),
                    Priority = 0.6
                }
            };

            foreach (var publishedPost in publishedPosts)
            {
                urls.Add(new SiteMapUrl
                {
                    Loc = publishedPost.FullUrlWithBaseAddress,
                    ChangeFreq = "daily",
                    LastMod = publishedPost.LastUpdateTimeUtc.ToChinaTime().ToDate(),
                    Priority = 0.5
                });
            }

            urlSet.Urls = urls;
            var xml = SerializeHelper.ToXml(urlSet, ns: "http://www.sitemaps.org/schemas/sitemap/0.9");
            return Content(xml, "text/xml", Encoding.UTF8);
        }
    }
}
