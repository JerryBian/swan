using Laobian.Blog.Models;
using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public PostController(IBlogService blogService, ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
        }

        [Route("{year:int}/{month:int}/{link}.html")]
        public IActionResult Index(int year, int month, string link)
        {
            var model = _cacheClient.GetOrCreate(
                CacheKey.Build(nameof(PostController), nameof(Index), year, month, link,
                    !User.Identity.IsAuthenticated),
                () =>
                {
                    var post = _blogService.GetPost(year, month, link, !User.Identity.IsAuthenticated);
                    if (post == null)
                    {
                        return null;
                    }

                    var viewModel = new PostViewModel { Post = post };
                    var posts = _blogService.GetPosts(!User.Identity.IsAuthenticated);
                    var postIndex = posts.IndexOf(post);
                    var nextPostIndex = postIndex - 1;
                    if (nextPostIndex >= 0)
                    {
                        viewModel.NextPost = posts[nextPostIndex];
                    }

                    var prevPostIndex = postIndex + 1;
                    if (prevPostIndex < posts.Count)
                    {
                        viewModel.NextPost = posts[prevPostIndex];
                    }

                    return viewModel;
                });

            if (model == null)
            {
                return NotFound();
            }

            if (!model.Post.IsPublic)
            {
                ViewData[ViewDataConstant.VisibleToSearchEngine] = false;
            }

            model.Post.NewAccess();
            ViewData[ViewDataConstant.Canonical] = model.Post.FullUrlWithBase;
            ViewData[ViewDataConstant.Title] = model.Post.Title;
            ViewData[ViewDataConstant.Description] = model.Post.ExcerptPlain;

            return View(model);
        }
    }
}