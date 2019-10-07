using Microsoft.AspNetCore.Mvc;
using Laobian.Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Laobian.Share.BlogEngine;
using Laobian.Share.Extension;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;

        public HomeController(IBlogService blogService)
        {
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
    }
}
