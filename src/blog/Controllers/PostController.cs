using System;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.BlogEngine;
using Laobian.Share.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;

        public PostController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [Route("{year:int}/{month:int}/{url}.html")]
        public IActionResult Index(int year, int month, string url)
        {
            var post = _blogService.GetPost(year, month, url);

            if (post == null)
            {
                return NotFound();
            }

            var categories = _blogService.GetCategories();
            var tags = _blogService.GetTags();
            var postViewModel = new PostViewModel(post);
            foreach (var blogPostCategoryName in post.CategoryNames)
            {
                var cat = categories.FirstOrDefault(_ =>
                    StringEqualsHelper.EqualsIgnoreCase(_.Name, blogPostCategoryName));
                if (cat != null)
                {
                    postViewModel.Categories.Add(cat);
                }
            }

            foreach (var blogPostTagName in post.TagNames)
            {
                var tag = tags.FirstOrDefault(_ =>
                    StringEqualsHelper.EqualsIgnoreCase(_.Name, blogPostTagName));
                if (tag != null)
                {
                    postViewModel.Tags.Add(tag);
                }
            }

            ViewData["Canonical"] = post.FullUrlWithBaseAddress;
            ViewData["Title"] = post.Title;
            ViewData["Description"] = post.ExcerptText;

            return View(postViewModel);
        }
    }
}
