using System;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.BlogEngine;
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
                    string.Equals(_.Name, blogPostCategoryName, StringComparison.OrdinalIgnoreCase));
                if (cat != null)
                {
                    postViewModel.Categories.Add(cat);
                }
            }

            foreach (var blogPostTagName in post.TagNames)
            {
                var tag = tags.FirstOrDefault(_ =>
                    string.Equals(_.Name, blogPostTagName, StringComparison.OrdinalIgnoreCase));
                if (tag != null)
                {
                    postViewModel.Tags.Add(tag);
                }
            }
            ViewData["robots"] = "index,follow,archive";
            ViewData["canonical"] = post.FullUrl;
            ViewData["Title"] = post.Title;

            return View(postViewModel);
        }
    }
}
