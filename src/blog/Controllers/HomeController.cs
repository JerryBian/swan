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
using Laobian.Blog.HttpClients;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiSiteHttpClient _apiSiteHttpClient;
        private readonly BlogOption _blogOption;
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IBlogService blogService,
            IOptions<BlogOption> config,
            ILogger<HomeController> logger,
            ApiSiteHttpClient apiSiteHttpClient,
            ICacheClient cacheClient)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _blogService = blogService;
            _blogOption = config.Value;
            _apiSiteHttpClient = apiSiteHttpClient;
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

            await _blogService.ReloadAsync();
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
                        _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished() || authenticated).ToList();
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
        [Route("/about")]
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
                        SystemAppVersion = _blogService.AppVersion,
                        SystemDotNetVersion = _blogService.RuntimeVersion,
                        SystemLastBoot = _blogService.BootTime.ToChinaDateAndTime(),
                        SystemRunningInterval = (DateTime.Now - _blogService.BootTime).ToDisplayString(),
                        TagTotalCount = _blogService.GetAllTags().Count.ToString(),
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
                foreach (var post in _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished()))
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
                    sb.AppendLine(
                        $"<url><loc>{_blogOption.BlogRemoteEndpoint}</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>");
                    sb.AppendLine(
                        $"<url><loc>{_blogOption.BlogRemoteEndpoint}/about</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");
                    sb.AppendLine(
                        $"<url><loc>{_blogOption.BlogRemoteEndpoint}/archive</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");
                    sb.AppendLine(
                        $"<url><loc>{_blogOption.BlogRemoteEndpoint}/tag</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");

                    foreach (var post in _blogService.GetAllPosts().Where(x => x.Raw.IsPostPublished()))
                    {
                        sb.AppendLine(
                            $"<url><loc>{post.Raw.GetFullPath(_blogOption.BlogRemoteEndpoint)}</loc><lastmod>{post.Raw.LastUpdateTime.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.6</priority></url>");
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