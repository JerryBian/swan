using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Misc;
using Laobian.Share.Model.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers;

public class HomeController : Controller
{
    private readonly BlogOptions _blogOptions;
    private readonly IBlogService _blogService;
    private readonly ICacheClient _cacheClient;

    public HomeController(
        IBlogService blogService,
        IOptions<BlogOptions> config,
        ICacheClient cacheClient)
    {
        _cacheClient = cacheClient;
        _blogService = blogService;
        _blogOptions = config.Value;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult Index([FromQuery] int page)
    {
        var authenticated = User.Identity?.IsAuthenticated ?? false;
        var viewModel = _cacheClient.GetOrCreate(
            CacheKeyBuilder.Build(nameof(HomeController), nameof(Index), page, authenticated),
            () =>
            {
                var posts =
                    _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished() || authenticated).ToList();
                var toppedPosts = posts.Where(x => x.Raw.IsTopping).ToList();
                foreach (var blogPost in toppedPosts)
                {
                    posts.Remove(blogPost);
                }

                posts.InsertRange(0, toppedPosts);

                var postsPerPage = Convert.ToInt32(_blogOptions.PostsPerPage);
                var model = new PagedViewModel<PostViewModel>(page, posts.Count, postsPerPage) {Url = Request.Path};

                foreach (var blogPost in posts.Chunk(postsPerPage).ElementAtOrDefault(model.CurrentPage - 1) ??
                                         Enumerable.Empty<BlogPostRuntime>())
                {
                    var postViewModel = new PostViewModel {Current = blogPost};
                    postViewModel.SetAdditionalInfo();
                    model.Items.Add(postViewModel);
                }

                return model;
            });

        if (viewModel.CurrentPage > 1)
        {
            ViewData["RobotsEnabled"] = false;
            ViewData["Title"] = $"首页：第{viewModel.CurrentPage}页";
        }

        return View(viewModel);
    }


    [HttpGet]
    [Route("/about")]
    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult About()
    {
        var authenticated = User.Identity?.IsAuthenticated ?? false;
        var viewModel = _cacheClient.GetOrCreate(
            CacheKeyBuilder.Build(nameof(HomeController), nameof(About), authenticated),
            () =>
            {
                var posts = _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished() || authenticated)
                    .ToList();
                var topTags = new Dictionary<BlogTag, int>();
                foreach (var tag in _blogService.GetAllTags())
                {
                    var count = posts.Count(x =>
                        x.Raw.Tag.Contains(tag.Id, StringComparer.InvariantCultureIgnoreCase));
                    topTags.Add(tag, count);
                }

                var postsPerPage = Convert.ToInt32(_blogOptions.PostsPerPage);
                var model = new AboutViewModel
                {
                    LatestPostRuntime = posts.FirstOrDefault(),
                    PostTotalAccessCount = posts.Sum(p => p.GetAccessCount()).ToThousandHuman(),
                    PostTotalCount = posts.Count.ToString(),
                    TopPosts = posts.OrderByDescending(p => p.GetAccessCount()).Take(postsPerPage),
                    SystemAppVersion = _blogOptions.AppVersion,
                    SystemDotNetVersion = _blogOptions.RuntimeVersion,
                    SystemLastBoot = _blogService.BootTime.ToChinaDateAndTime(),
                    SystemRunningInterval = (DateTime.Now - _blogService.BootTime).ToHuman(),
                    TagTotalCount = _blogService.GetAllTags().Count.ToString(),
                    TopTags = topTags.OrderByDescending(x => x.Value).Take(postsPerPage)
                        .ToDictionary(x => x.Key, x => x.Value)
                };

                return model;
            });


        return View("~/Views/About/Index.cshtml", viewModel);
    }

    [Route("/rss")]
    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult Rss()
    {
        var rss = _cacheClient.GetOrCreate(CacheKeyBuilder.Build(nameof(HomeController), nameof(Rss)), () =>
        {
            var feed = new SyndicationFeed(Constants.BlogTitle, Constants.BlogDescription,
                new Uri($"{_blogOptions.BlogRemoteEndpoint}/rss"),
                Constants.ApplicationName, DateTimeOffset.UtcNow)
            {
                Copyright = new TextSyndicationContent(
                    $"&#x26;amp;#169; {DateTime.Now.Year} {_blogOptions.AdminChineseName}")
            };
            feed.Authors.Add(new SyndicationPerson(_blogOptions.AdminEmail,
                _blogOptions.AdminChineseName,
                _blogOptions.BlogRemoteEndpoint));
            feed.BaseUri = new Uri(_blogOptions.BlogRemoteEndpoint);
            feed.Language = "zh-cn";
            var items = new List<SyndicationItem>();
            foreach (var post in _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished()))
            {
                items.Add(new SyndicationItem(post.Raw.Title, post.HtmlContent,
                    new Uri(post.Raw.GetFullPath(_blogOptions.BlogRemoteEndpoint)),
                    post.Raw.GetFullPath(_blogOptions.BlogRemoteEndpoint),
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

            using var ms = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(ms, settings))
            {
                var rssFormatter = new Rss20FeedFormatter(feed, false);
                rssFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        });

        return Content(rss, "application/rss+xml", Encoding.UTF8);
    }

    [Route("/sitemap")]
    [Route("/sitemap.xml")]
    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult Sitemap()
    {
        var sitemap = _cacheClient.GetOrCreate(CacheKeyBuilder.Build(nameof(HomeController), nameof(Sitemap)),
            () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
                sb.AppendLine(
                    $"<url><loc>{_blogOptions.BlogRemoteEndpoint}</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>");
                sb.AppendLine(
                    $"<url><loc>{_blogOptions.BlogRemoteEndpoint}/read</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");
                sb.AppendLine(
                    $"<url><loc>{_blogOptions.BlogRemoteEndpoint}/about</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");
                sb.AppendLine(
                    $"<url><loc>{_blogOptions.BlogRemoteEndpoint}/archive</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");
                sb.AppendLine(
                    $"<url><loc>{_blogOptions.BlogRemoteEndpoint}/tag</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");

                foreach (var post in _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished()))
                {
                    sb.AppendLine(
                        $"<url><loc>{post.Raw.GetFullPath(_blogOptions.BlogRemoteEndpoint)}</loc><lastmod>{post.Raw.LastUpdateTime.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.6</priority></url>");
                }

                sb.AppendLine("</urlset>");
                return sb.ToString();
            });
        return Content(sitemap, "text/xml", Encoding.UTF8);
    }

    [HttpGet]
    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}