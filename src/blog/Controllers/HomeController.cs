﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Encodings.Web;
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
        private readonly BlogOption _blogOption;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<HomeController> _logger;
        private readonly ISystemData _systemData;

        public HomeController(
            ISystemData systemData,
            IOptions<BlogOption> config,
            ILogger<HomeController> logger,
            ApiHttpService apiHttpService,
            ICacheClient cacheClient)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _systemData = systemData;
            _blogOption = config.Value;
            _apiHttpService = apiHttpService;
        }

        [HttpPost]
        [Route("/reload")]
        public async Task<IActionResult> Reload()
        {
            if (!HttpContext.Request.Headers.ContainsKey(Constants.ApiRequestHeaderToken))
            {
                return BadRequest("No API token set.");
            }

            if (_blogOption.HttpRequestToken != HttpContext.Request.Headers[Constants.ApiRequestHeaderToken])
            {
                return BadRequest(
                    $"Invalid API token set: {HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}");
            }

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
                        _systemData.Posts.Where(x => authenticated || x.Raw.IsPostPublished())
                            .OrderByDescending(x => x.Raw.PublishTime).ToList();
                    var toppedPosts = posts.Where(x => x.Raw.IsTopping).ToList();
                    foreach (var blogPost in toppedPosts)
                    {
                        posts.Remove(blogPost);
                    }

                    posts.InsertRange(0, toppedPosts);

                    var postsPerPage = Convert.ToInt32(_blogOption.PostsPerPage);
                    var model = new PagedPostViewModel(p, posts.Count, postsPerPage) {Url = Request.Path};

                    foreach (var blogPost in posts.ToPaged(postsPerPage, model.CurrentPage))
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
                    var posts = _systemData.Posts.Where(x => authenticated || x.Raw.IsPostPublished())
                        .OrderByDescending(x => x.Raw.PublishTime).ToList();
                    var model = new List<PostArchiveViewModel>();
                    foreach (var item in posts.GroupBy(x => x.Raw.PublishTime.Year).OrderByDescending(y => y.Key))
                    {
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = item.Count(),
                            Posts = item.OrderByDescending(x => x.Raw.PublishTime).ToList(),
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
                    var posts = _systemData.Posts.Where(x => authenticated || x.Raw.IsPostPublished())
                        .OrderByDescending(x => x.Raw.PublishTime).ToList();
                    var model = new List<PostArchiveViewModel>();

                    foreach (var blogTag in tags.OrderByDescending(x => x.LastUpdatedAt))
                    {
                        var tagPosts = posts.Where(x => x.Raw.Tag.Contains(blogTag.Link)).ToList();
                        var archiveViewModel = new PostArchiveViewModel
                        {
                            Count = tagPosts.Count(),
                            Posts = tagPosts.OrderByDescending(x => x.Raw.PublishTime).ToList(),
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
        public IActionResult Post([FromRoute] int year, [FromRoute] int month, [FromRoute] string link)
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Post), authenticated, year, month, link),
                () =>
                {
                    var post = _systemData.Posts.FirstOrDefault(x =>
                        StringUtil.EqualsIgnoreCase(x.Raw.Link, link) &&
                        x.Raw.PublishTime.Year == year &&
                        x.Raw.PublishTime.Month == month &&
                        (x.Raw.IsPublished || authenticated));
                    if (post == null)
                    {
                        return null;
                    }

                    var previousPost = _systemData.Posts.OrderByDescending(x => x.Raw.PublishTime)
                        .FirstOrDefault(x => x.Raw.PublishTime < post.Raw.PublishTime);
                    var nextPost = _systemData.Posts.OrderBy(x => x.Raw.PublishTime)
                        .FirstOrDefault(x => x.Raw.PublishTime > post.Raw.PublishTime);
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
            Task.Run(() => _apiHttpService.AddPostAccess(viewModel.Current.Raw.Link));
#pragma warning restore 4014

            return View("~/Views/Post/Index.cshtml", viewModel);
        }

        [HttpGet]
        [Route("/about")]
        public IActionResult About()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var viewModel = _cacheClient.GetOrCreate(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(About), authenticated),
                () =>
                {
                    var posts = _systemData.Posts.Where(x => authenticated || x.Raw.IsPostPublished())
                        .OrderByDescending(x => x.Raw.PublishTime).ToList();
                    var tags = _systemData.Tags;
                    var topTags = new Dictionary<BlogTag, int>();
                    foreach (var tag in tags)
                    {
                        var count = posts.Count(x =>
                            x.Raw.Tag.Contains(tag.Link, StringComparer.InvariantCultureIgnoreCase));
                        topTags.Add(tag, count);
                    }

                    var postsPerPage = Convert.ToInt32(_blogOption.PostsPerPage);
                    var model = new AboutViewModel
                    {
                        LatestPostRuntime = posts.FirstOrDefault(),
                        PostTotalAccessCount = posts.Sum(p => p.GetAccessCount()).ToUSThousand(),
                        PostTotalCount = posts.Count.ToString(),
                        TopPosts = posts.OrderByDescending(p => p.GetAccessCount()).Take(postsPerPage),
                        SystemAppVersion = _systemData.AppVersion,
                        SystemDotNetVersion = _systemData.RuntimeVersion,
                        SystemLastBoot = _systemData.BootTime.ToChinaDateAndTime(),
                        SystemRunningInterval = (DateTime.Now - _systemData.BootTime).ToDisplayString(),
                        TagTotalCount = tags.Count.ToString(),
                        TopTags = topTags.OrderByDescending(x => x.Value).Take(postsPerPage)
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
                    new Uri($"{_blogOption.BlogRemoteEndpoint}/rss"),
                    Constants.ApplicationName, DateTimeOffset.UtcNow);
                feed.Copyright =
                    new TextSyndicationContent($"&#x26;amp;#169; {DateTime.Now.Year} {_blogOption.AdminChineseName}");
                feed.Authors.Add(new SyndicationPerson(_blogOption.AdminEmail, _blogOption.AdminChineseName,
                    _blogOption.BlogRemoteEndpoint));
                feed.BaseUri = new Uri(_blogOption.BlogRemoteEndpoint);
                feed.Language = "zh-cn";
                var items = new List<SyndicationItem>();
                foreach (var post in _systemData.Posts.Where(x => x.Raw.IsPostPublished()))
                {
                    items.Add(new SyndicationItem(post.Raw.Title, post.HtmlContent,
                        new Uri(post.Raw.GetFullPath(_blogOption.BlogRemoteEndpoint)),
                        post.Raw.GetFullPath(_blogOption.BlogRemoteEndpoint),
                        new DateTimeOffset(post.Raw.LastUpdateTime, TimeSpan.FromHours(8))));
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

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        public IActionResult Sitemap()
        {
            var sitemap = _cacheClient.GetOrCreate(CacheKeyBuilder.Build(nameof(HomeController), nameof(Sitemap)),
                () =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
                    sb.AppendLine($"<url><loc>{_blogOption.BlogRemoteEndpoint}</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>");
                    sb.AppendLine($"<url><loc>{_blogOption.BlogRemoteEndpoint}/about</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");
                    sb.AppendLine($"<url><loc>{_blogOption.BlogRemoteEndpoint}/archive</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");
                    sb.AppendLine($"<url><loc>{_blogOption.BlogRemoteEndpoint}/tag</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");

                    foreach (var post in _systemData.Posts.Where(x => x.Raw.IsPostPublished()))
                    {
                        sb.AppendLine($"<url><loc>{post.Raw.GetFullPath(_blogOption.BlogRemoteEndpoint)}</loc><lastmod>{post.Raw.LastUpdateTime.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.6</priority></url>");
                    }

                    sb.AppendLine("</urlset>");
                    return sb.ToString();
                });
            return Content(sitemap, "text/xml", Encoding.UTF8);
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