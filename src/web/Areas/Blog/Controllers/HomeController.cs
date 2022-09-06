using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Extension;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Laobian.Web.Areas.Blog.Controllers
{
    [Area(Constants.AreaBlog)]
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly LaobianOption _option;

        public HomeController(IBlogService blogService, IOptions<LaobianOption> option)
        {
            _option = option.Value;
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            List<BlogPostView> items = await _blogService.GetAllPostsAsync();
            if (!isAuthenticated)
            {
                items = items.Where(x => x.Raw.IsPublic).ToList();
            }

            var model = items.OrderByDescending(x => x.Raw.PublishTime).ToList();
            ViewData["Title"] = "博客";
            return View(model);
        }

        [Route("/blog/rss")]
        [Route("/blog/feed")]
        [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
        public async Task<IActionResult> Rss()
        {
            var feed = new SyndicationFeed(_option.Title, _option.Description,
                    new Uri($"{_option.BaseUrl}/blog/rss"),
                    _option.AppName, DateTimeOffset.UtcNow)
            {
                Copyright = new TextSyndicationContent(
                        $"&#x26;amp;#169; {DateTime.Now.Year} {_option.AdminUserFullName}")
            };
            feed.Authors.Add(new SyndicationPerson(_option.AdminEmail,
                _option.AdminUserFullName,
                _option.BaseUrl));
            feed.BaseUri = new Uri(_option.BaseUrl);
            feed.Language = "zh-cn";
            var items = new List<SyndicationItem>();
            var posts = await _blogService.GetAllPostsAsync();
            foreach (var post in posts.Where(x => x.IsPublished()))
            {
                items.Add(new SyndicationItem(post.Raw.Title, post.HtmlContent,
                    new Uri($"{_option.BaseUrl}{post.FullLink}"),
                    $"{_option.BaseUrl}{post.FullLink}",
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

            var rss = Encoding.UTF8.GetString(ms.ToArray());
            return Content(rss, "application/rss+xml", Encoding.UTF8);
        }
    }
}
