using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.BlogEngine;
using Laobian.Share.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<PostController> _logger;

        public PostController(IBlogService blogService, ILogger<PostController> logger)
        {
            _logger = logger;
            _blogService = blogService;
        }

        [Route("{year:int}/{month:int}/{url}.html")]
        public IActionResult Index(int year, int month, string url)
        {
            var post = _blogService.GetPost(year, month, url);

            if (post == null)
            {
                _logger.LogWarning("Request post not exists. {Year}, {Month}, {Link}", year, month, url);
                return NotFound();
            }

            if (!post.IsPublic)
            {
                ViewData["Robots"] = "noindex, nofollow";

                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Trying to access private post failed. {Year}, {Month}, {Link}", year, month, url);
                    return NotFound();
                }
            }




            var categories = _blogService.GetCategories();
            var tags = _blogService.GetTags();
            var postViewModel = new PostViewModel(post);
            foreach (var blogPostCategoryName in post.CategoryNames)
            {
                var cat = categories.FirstOrDefault(_ =>
                    StringEqualsHelper.IgnoreCase(_.Name, blogPostCategoryName));
                if (cat != null)
                {
                    postViewModel.Categories.Add(cat);
                }
            }

            foreach (var blogPostTagName in post.TagNames)
            {
                var tag = tags.FirstOrDefault(_ =>
                    StringEqualsHelper.IgnoreCase(_.Name, blogPostTagName));
                if (tag != null)
                {
                    postViewModel.Tags.Add(tag);
                }
            }

            var posts = User.Identity.IsAuthenticated
                ? _blogService.GetPosts().OrderByDescending(p => p.CreationTimeUtc).ToList()
                : _blogService.GetPosts().Where(p => p.IsPublic).OrderByDescending(p => p.CreationTimeUtc).ToList();
            var postIndex = posts.IndexOf(post);
            if (postIndex > 0)
            {
                postViewModel.NextPost = posts[postIndex - 1];
            }

            if (postIndex < posts.Count - 1)
            {
                postViewModel.PrevPost = posts[postIndex + 1];
            }

            ViewData["Canonical"] = post.FullUrlWithBaseAddress;
            ViewData["Title"] = post.Title;
            ViewData["Description"] = post.ExcerptText;
            ViewData["AdminView"] = HttpContext.User.Identity.IsAuthenticated;

            return View(postViewModel);
        }
    }
}
