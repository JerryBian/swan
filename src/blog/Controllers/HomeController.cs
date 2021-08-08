using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Laobian.Blog.Cache;
using Laobian.Blog.Data;
using Laobian.Blog.HttpService;
using Laobian.Blog.Models;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Extension;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly BlogConfig _blogConfig;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<HomeController> _logger;
        private readonly ISystemData _systemData;

        public HomeController(
            ISystemData systemData,
            IOptions<BlogConfig> config,
            ILogger<HomeController> logger,
            ApiHttpService apiHttpService,
            ICacheClient cacheClient)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _systemData = systemData;
            _blogConfig = config.Value;
            _apiHttpService = apiHttpService;
        }

        [HttpPost]
        [Route("/reload")]
        public async Task<IActionResult> Reload()
        {
            //TODO: we need to verify request. Hard for containers
            await _systemData.LoadAsync();
            return Ok();
        }

        [HttpGet]
        public IActionResult Index([FromQuery] int p)
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Index), p, authenticated),
                () =>
                {
                    var posts =
                        _systemData.Posts.Where(x => authenticated || x.IsPublished)
                            .OrderByDescending(x => x.Metadata.PublishTime).ToList();
                    var toppedPosts = posts.Where(x => x.Metadata.IsTopping).ToList();
                    foreach (var blogPost in toppedPosts)
                    {
                        posts.Remove(blogPost);
                    }

                    posts.InsertRange(0, toppedPosts);

                    var model = new PagedPostViewModel(p, posts.Count, _blogConfig.PostsPerPage) {Url = Request.Path};

                    foreach (var blogPost in posts.ToPaged(_blogConfig.PostsPerPage, model.CurrentPage))
                    {
                        var postViewModel = new PostViewModel {Current = blogPost};
                        postViewModel.SetAdditionalInfo();
                        model.Posts.Add(postViewModel);
                    }

                    return model;
                });


            return View(viewModel);
        }

        [HttpGet]
        [Route("/archive")]
        public IActionResult Archive()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Archive), authenticated),
                () =>
                {
                    var posts = _systemData.Posts.Where(x => authenticated || x.IsPublished)
                        .OrderByDescending(x => x.Metadata.PublishTime).ToList();
                    var model = new List<PostArchiveViewModel>();
                    foreach (var item in posts.GroupBy(x => x.Metadata.PublishTime.Year).OrderByDescending(y => y.Key))
                    {
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = item.Count(),
                            Posts = item.OrderByDescending(x => x.Metadata.PublishTime).ToList(),
                            Link = $"{item.Key}",
                            Name = $"{item.Key}年",
                            BaseUrl = "/archive"
                        };

                        model.Add(archiveViewModel);
                    }

                    return model;
                });

            ViewData["Title"] = "存档";
            return View("~/Views/Archive/Index.cshtml", viewModel);
        }

        [HttpGet]
        [Route("/tag")]
        public IActionResult Tag()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Tag), authenticated),
                () =>
                {
                    var tags = _systemData.Tags;
                    var posts = _systemData.Posts.Where(x => authenticated || x.IsPublished)
                        .OrderByDescending(x => x.Metadata.PublishTime).ToList();
                    var model = new List<PostArchiveViewModel>();

                    foreach (var blogTag in tags.OrderByDescending(x => x.LastUpdatedAt))
                    {
                        var tagPosts = posts.Where(x => x.Metadata.Tags.Contains(blogTag.Link)).ToList();
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = tagPosts.Count(),
                            Posts = tagPosts.OrderByDescending(x => x.Metadata.PublishTime).ToList(),
                            Link = $"{blogTag.Link}",
                            Name = $"{blogTag.DisplayName}",
                            BaseUrl = "/tag"
                        };
                        model.Add(archiveViewModel);
                    }

                    return model;
                });

            ViewData["Title"] = "标签";
            return View("~/Views/Archive/Index.cshtml", viewModel);
        }

        [HttpGet]
        [Route("/{year:int}/{month:int}/{link}.html")]
        public async Task<IActionResult> Post([FromRoute] int year, [FromRoute] int month, [FromRoute] string link)
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Post), authenticated, year, month, link),
                () =>
                {
                    var post = _systemData.Posts.FirstOrDefault(x =>
                        StringUtil.EqualsIgnoreCase(x.Link, link) &&
                        x.Metadata.PublishTime.Year == year &&
                        x.Metadata.PublishTime.Month == month &&
                        (x.Metadata.IsPublished || authenticated));
                    if (post == null)
                    {
                        return null;
                    }

                    var previousPost = _systemData.Posts.OrderByDescending(x => x.Metadata.PublishTime)
                        .FirstOrDefault(x => x.Metadata.PublishTime < post.Metadata.PublishTime);
                    var nextPost = _systemData.Posts.OrderBy(x => x.Metadata.PublishTime)
                        .FirstOrDefault(x => x.Metadata.PublishTime > post.Metadata.PublishTime);
                    var model = new PostViewModel
                    {
                        Current = post,
                        Previous = previousPost,
                        Next = nextPost
                    };
                    model.SetAdditionalInfo();
                    return model;
                });

            if (viewModel == null)
            {
                return NotFound();
            }

#pragma warning disable 4014
            Task.Run(() => _apiHttpService.AddPostAccess(viewModel.Current.Link));
#pragma warning restore 4014

            return View("~/Views/Post/Index.cshtml", viewModel);
        }

        [HttpGet]
        [Route("/about")]
        public async Task<IActionResult> About()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(About), authenticated),
                () =>
                {
                    var posts = _systemData.Posts.Where(x => authenticated || x.IsPublished).ToList();
                    var tags = _systemData.Tags;
                    var topTags = new Dictionary<BlogTag, int>();
                    foreach (var tag in tags)
                    {
                        var count = posts.Count(x =>
                            x.Metadata.Tags.Contains(tag.Link, StringComparer.InvariantCultureIgnoreCase));
                        topTags.Add(tag, count);
                    }

                    var model = new AboutViewModel
                    {
                        LatestPost = posts.FirstOrDefault(),
                        PostTotalAccessCount = posts.Sum(p => p.Accesses.Count).ToUSThousand(),
                        PostTotalCount = posts.Count.ToString(),
                        TopPosts = posts.OrderByDescending(p => p.Accesses.Count).Take(_blogConfig.PostsPerPage),
                        SystemAppVersion = _systemData.AppVersion,
                        SystemDotNetVersion = _systemData.RuntimeVersion,
                        SystemLastBoot = _systemData.BootTime.ToChinaDateAndTime(),
                        SystemRunningInterval = (DateTime.Now - _systemData.BootTime).ToString(),
                        TagTotalCount = tags.Count.ToString(),
                        TopTags = topTags.OrderByDescending(x => x.Value).Take(_blogConfig.PostsPerPage)
                            .ToDictionary(x => x.Key, x => x.Value)
                    };

                    return model;
                });


            return View("~/Views/About/Index.cshtml", viewModel);
        }

        [Route("/rss")]
        public IActionResult Rss()
        {
            var rss = _cacheClient.GetOrCreate(CacheKeyBuilder.Build(nameof(HomeController), nameof(Rss)), () =>
            {
                var feed = new SyndicationFeed(Constants.BlogTitle, Constants.BlogDescription,
                    new Uri($"{Constants.BlogAddress}/rss"),
                    Constants.ApplicationName, DateTimeOffset.UtcNow);
                feed.Copyright = new TextSyndicationContent($"&#x26;amp;#169; {DateTime.Now.Year} {Constants.AdminChineseName}");
                feed.Authors.Add(new SyndicationPerson(Constants.AdminEmail, Constants.AdminChineseName,
                    Constants.BlogAddress));
                feed.BaseUri = new Uri(Constants.BlogAddress);
                feed.Language = "zh-cn";
                var items = new List<SyndicationItem>();
                foreach (var post in _systemData.Posts.Where(x => x.IsPublished))
                {
                    items.Add(new SyndicationItem(post.Metadata.Title, post.HtmlContent,
                        new Uri(post.GetFullPath(true)), post.GetFullPath(),
                        new DateTimeOffset(post.Metadata.LastUpdateTime, TimeSpan.FromHours(8))));
                }

                feed.Items = items;
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    NewLineHandling = NewLineHandling.Entitize,
                    NewLineOnAttributes = false,
                    Async = true,
                    Indent = true
                };

                using (var ms = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(ms, settings))
                    {
                        var rssFormatter = new Rss20FeedFormatter(feed, false);
                        rssFormatter.WriteTo(xmlWriter);
                        xmlWriter.Flush();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            });

            return Content(rss, "application/rss+xml", Encoding.UTF8);
        }

        [HttpGet]
        [HttpPost]
        [HttpPut]
        [Route("/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}