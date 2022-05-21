using System.Linq;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Laobian.Share;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers;

public class PostController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ICacheClient _cacheClient;

    public PostController(ICacheClient cacheClient, IBlogService blogService)
    {
        _cacheClient = cacheClient;
        _blogService = blogService;
    }

    [HttpGet]
    [Route("/{year}/{month}/{link}.html")]
    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult Post([FromRoute] int year, [FromRoute] int month, [FromRoute] string link)
    {
        var authenticated = User.Identity?.IsAuthenticated ?? false;
        var viewModel = _cacheClient.GetOrCreate(
            CacheKeyBuilder.Build(nameof(HomeController), nameof(Post), authenticated, link),
            () =>
            {
                var post = _blogService.GetAllPosts().FirstOrDefault(x =>
                    StringUtil.EqualsIgnoreCase(x.Raw.Link, link) &&
                    (x.Raw.IsPostPublished() || authenticated));
                if (post == null)
                {
                    return null;
                }

                var previousPost = _blogService.GetAllPosts()
                    .FirstOrDefault(x => x.Raw.PublishTime < post.Raw.PublishTime);
                var nextPost = _blogService.GetAllPosts()
                    .LastOrDefault(x => x.Raw.PublishTime > post.Raw.PublishTime);
                var model = new PostViewModel
                {
                    Current = post,
                    Previous = previousPost,
                    Next = nextPost
                };
                model.SetAdditionalInfo();
                return model;
            });

        if (viewModel == null || viewModel.Current.Raw.PublishTime.Year != year ||
            viewModel.Current.Raw.PublishTime.Month != month)
        {
            return NotFound();
        }

        _blogService.EnqueuePostAccess(viewModel.Current.Raw.Link);
        return View("Index", viewModel);
    }
}