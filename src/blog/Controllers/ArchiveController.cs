using System;
using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public ArchiveController(IOptions<AppConfig> appConfig, IBlogService blogService)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        [Route("/category")]
        [ResponseCache(CacheProfileName = "Cache10Sec")]
        public IActionResult Category()
        {
            var model = new List<ArchiveViewModel>();
            var posts = _blogService.GetPublishedPosts();
            var cats = _blogService.GetCategories();
            foreach (var blogCategory in cats)
            {
                var catModel = new ArchiveViewModel(blogCategory.Name, blogCategory.Link);
                catModel.Posts.AddRange(posts.Where(p => p.CategoryNames.Contains(blogCategory.Name, StringComparer.OrdinalIgnoreCase)));

                model.Add(catModel);
            }

            var remainingPosts = posts.Except(model.SelectMany(_ => _.Posts).Distinct()).ToList();
            if (remainingPosts.Any())
            {
                var catModel = new ArchiveViewModel(_appConfig.Blog.DefaultCategoryName, _appConfig.Blog.DefaultCategoryLink);
                catModel.Posts.AddRange(remainingPosts);
                model.Add(catModel);
            }

            ViewData["Title"] = "分类";
            ViewData["Canonical"] = "/category/";
            ViewData["Description"] = "所有文章以分类的形式展现";
            return View("Index", model);
        }

        [Route("/tag")]
        [ResponseCache(CacheProfileName = "Cache10Sec")]
        public IActionResult Tag()
        {
            var model = new List<ArchiveViewModel>();
            var posts = _blogService.GetPublishedPosts();
            var tags = _blogService.GetTags();
            foreach (var tag in tags)
            {
                var tagModel = new ArchiveViewModel(tag.Name, tag.Link);
                tagModel.Posts.AddRange(posts.Where(p => p.TagNames.Contains(tag.Name, StringComparer.OrdinalIgnoreCase)));

                model.Add(tagModel);
            }

            var remainingPosts = posts.Except(model.SelectMany(_ => _.Posts).Distinct()).ToList();
            if (remainingPosts.Any())
            {
                var tagModel = new ArchiveViewModel(_appConfig.Blog.DefaultTagName, _appConfig.Blog.DefaultTagLink);
                tagModel.Posts.AddRange(remainingPosts);
                model.Add(tagModel);
            }

            ViewData["Title"] = "标签";
            ViewData["Canonical"] = "/tag/";
            ViewData["Description"] = "所有文章以标签归类的形式展现";
            return View("Index", model);
        }

        [Route("/archive")]
        [ResponseCache(CacheProfileName = "Cache10Sec")]
        public IActionResult Date()
        {
            var model = new List<ArchiveViewModel>();
            var posts = _blogService.GetPublishedPosts();
            var dates = posts.Select(_ => _.CreationTimeUtc.Year).Distinct();
            foreach (var date in dates)
            {
                var catModel = new ArchiveViewModel($"{date} 年", date.ToString());
                catModel.Posts.AddRange(posts.Where(p => p.CreationTimeUtc.Year == date).OrderByDescending(p => p.CreationTimeUtc));

                model.Add(catModel);
            }

            ViewData["Title"] = "存档";
            ViewData["Canonical"] = "/archive/";
            ViewData["Description"] = "所有文章以发表日期归类的形式展现";
            return View("Index", model.OrderByDescending(m => m.Name));
        }
    }
}