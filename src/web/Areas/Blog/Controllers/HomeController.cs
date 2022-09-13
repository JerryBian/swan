using Laobian.Lib;
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

            List<BlogPostView> model = items.OrderByDescending(x => x.Raw.PublishTime).ToList();
            ViewData["Title"] = "博客";
            return View(model);
        }

        [Route("/blog/rss")]
        [Route("/blog/feed")]
        [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
        public async Task<IActionResult> Rss()
        {
            SyndicationFeed feed = new(_option.Title, _option.Description,
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
            List<SyndicationItem> items = new();
            List<BlogPostView> posts = await _blogService.GetAllPostsAsync();
            foreach (BlogPostView post in posts.Where(x => x.IsPublished()))
            {
                items.Add(new SyndicationItem(post.Raw.Title, post.HtmlContent,
                    new Uri($"{_option.BaseUrl}{post.FullLink}"),
                    $"{_option.BaseUrl}{post.FullLink}",
                    new DateTimeOffset(post.Raw.LastUpdateTime, TimeSpan.FromHours(8))));
            }

            feed.Items = items;
            XmlWriterSettings settings = new()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = false,
                Async = true,
                Indent = true
            };

            using MemoryStream ms = new();
            using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
            {
                Rss20FeedFormatter rssFormatter = new(feed, false);
                rssFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }

            string rss = Encoding.UTF8.GetString(ms.ToArray());
            return Content(rss, "application/rss+xml", Encoding.UTF8);
        }
    }
}
