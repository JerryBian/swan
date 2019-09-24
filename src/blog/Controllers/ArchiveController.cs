using System;
using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.BlogEngine;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly IBlogService _blogService;

        public ArchiveController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [Route("/category")]
        public IActionResult Category()
        {
            var model = new List<ArchiveViewModel>();
            var posts = _blogService.GetPosts().Where(_ => _.IsPublic).ToList();
            var cats = _blogService.GetCategories();
            foreach (var blogCategory in cats)
            {
                var catModel = new ArchiveViewModel(blogCategory.Name, blogCategory.Link);
                catModel.Posts.AddRange(posts.Where(p=>p.CategoryNames.Contains(blogCategory.Name, StringComparer.OrdinalIgnoreCase)));

                model.Add(catModel);
            }

            ViewData["Title"] = "分类";
            return View("Index", model);
        }

        [Route("/tag")]
        public IActionResult Tag()
        {
            var model = new List<ArchiveViewModel>();
            var posts = _blogService.GetPosts().Where(_ => _.IsPublic).ToList();
            var tags = _blogService.GetTags();
            foreach (var tag in tags)
            {
                var catModel = new ArchiveViewModel(tag.Name, tag.Link);
                catModel.Posts.AddRange(posts.Where(p => p.TagNames.Contains(tag.Name, StringComparer.OrdinalIgnoreCase)));

                model.Add(catModel);
            }

            ViewData["Title"] = "标签";
            return View("Index", model);
        }

        [Route("/archive")]
        public IActionResult Date()
        {
            var model = new List<ArchiveViewModel>();
            var posts = _blogService.GetPosts().Where(_ => _.IsPublic).ToList();
            var dates = posts.Select(_ => _.CreationTimeUtc.Year).Distinct();
            foreach (var date in dates)
            {
                var catModel = new ArchiveViewModel($"{date} 年", date.ToString());
                catModel.Posts.AddRange(posts.Where(p => p.CreationTimeUtc.Year == date));

                model.Add(catModel);
            }

            ViewData["Title"] = "存档";
            return View("Index", model);
        }
    }
}