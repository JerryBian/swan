using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Laobian.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly LaobianBlogOption _laobianBlogOption;
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public ArchiveController(ICacheClient cacheClient, IBlogService blogService, IOptions<LaobianBlogOption> blogOption)
        {
            _cacheClient = cacheClient;
            _blogService = blogService;
            _laobianBlogOption = blogOption.Value;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
        public IActionResult Index()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(ArchiveController), nameof(Index), authenticated),
                () =>
                {
                    var posts = _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished() || authenticated)
                        .ToList();
                    var model = new List<PostArchiveViewModel>();
                    foreach (var item in posts.GroupBy(x => x.Raw.PublishTime.Year).OrderByDescending(y => y.Key))
                    {
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = item.Count(),
                            Posts = item.ToList(),
                            Link = $"{item.Key}",
                            Name = $"{item.Key}年"
                        };

                        model.Add(archiveViewModel);
                    }

                    return model;
                });

            ViewData["Title"] = "存档";
            ViewData["Image"] = $"{_laobianBlogOption.BlogRemoteEndpoint}/archive.png";
            return View("Index", viewModel);
        }
    }
}