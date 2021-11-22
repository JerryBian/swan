using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Laobian.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers;

public class TagController : Controller
{
    private readonly BlogOptions _blogOptions;
    private readonly IBlogService _blogService;
    private readonly ICacheClient _cacheClient;

    public TagController(ICacheClient cacheClient, IBlogService blogService, IOptions<BlogOptions> blogOption)
    {
        _cacheClient = cacheClient;
        _blogService = blogService;
        _blogOptions = blogOption.Value;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult Index()
    {
        var authenticated = User.Identity?.IsAuthenticated ?? false;
        var viewModel = _cacheClient.GetOrCreate(
            CacheKeyBuilder.Build(nameof(HomeController), nameof(Index), authenticated),
            () =>
            {
                var posts = _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished() || authenticated)
                    .ToList();
                var model = new List<PostArchiveViewModel>();

                foreach (var blogTag in _blogService.GetAllTags())
                {
                    var tagPosts = posts.Where(x => x.Raw.Tag.Contains(blogTag.Link)).ToList();
                    if (tagPosts.Any())
                    {
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = tagPosts.Count,
                            Posts = tagPosts.ToList(),
                            Link = $"{blogTag.Link}",
                            Name = $"{blogTag.DisplayName}"
                        };

                        model.Add(archiveViewModel);
                    }
                }

                return model;
            });

        ViewData["Title"] = "标签";
        ViewData["Image"] = $"{_blogOptions.BlogRemoteEndpoint}/archive.png";
        return View(viewModel);
    }
}