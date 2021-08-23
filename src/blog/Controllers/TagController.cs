using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class TagController : Controller
    {
        private readonly BlogOption _blogOption;
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public TagController(ICacheClient cacheClient, IBlogService blogService, IOptions<BlogOption> blogOption)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
            _blogOption = blogOption.Value;
        }

        [HttpGet]
        public IActionResult Tag()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Tag), authenticated),
                () =>
                {
                    var posts = _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished() || authenticated)
                        .ToList();
                    var model = new List<PostArchiveViewModel>();

                    foreach (var blogTag in _blogService.GetAllTags())
                    {
                        var tagPosts = posts.Where(x => x.Raw.Tag.Contains(blogTag.Link)).ToList();
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = tagPosts.Count,
                            Posts = tagPosts.ToList(),
                            Link = $"{blogTag.Link}",
                            Name = $"{blogTag.DisplayName}",
                            BaseUrl = "/tag"
                        };
                        model.Add(archiveViewModel);
                    }

                    return model;
                });

            ViewData["Title"] = "标签";
            ViewData["Image"] = $"{_blogOption.BlogRemoteEndpoint}/archive.png";
            return View("~/Views/Archive/Index.cshtml", viewModel);
        }
    }
}