using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Laobian.Share.BlogEngine;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly IBlogService _blogService;

        public SubscribeController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/rss")]
        public async Task<IActionResult> Rss()
        {
            var feed = await GetFeedAsync(BlogConstant.RssLink);
            var rssFormatter = new Rss20FeedFormatter(feed);
            using (var ms = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(ms,new XmlWriterSettings{Async = true, Encoding = Encoding.UTF8}))
                {
                    rssFormatter.WriteTo(xmlWriter);
                }

                return Content(Encoding.UTF8.GetString(ms.ToArray()), "application/rss+xml", Encoding.UTF8);
            }
        }

        [Route("/atom")]
        public async Task<IActionResult> Atom()
        {
            var feed = await GetFeedAsync(BlogConstant.AtomLink);
            var atomFormatter = new Atom10FeedFormatter(feed);
            using (var ms = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8 }))
                {
                    atomFormatter.WriteTo(xmlWriter);
                }

                return Content(Encoding.UTF8.GetString(ms.ToArray()), "application/atom+xml", Encoding.UTF8);
            }
        }

        private async Task<SyndicationFeed> GetFeedAsync(string alterLink)
        {
            var feed = new SyndicationFeed(BlogConstant.BlogName, BlogConstant.BlogDescription, new Uri(alterLink), BlogConstant.WebAddress, DateTimeOffset.UtcNow);
            var sp = new SyndicationPerson(BlogConstant.AuthorEmail, BlogConstant.AuthorChineseName, BlogConstant.WebAddress);
            feed.Authors.Add(sp);

            feed.Contributors.Add(sp);
            feed.Copyright = new TextSyndicationContent($"Copyright 2008-{DateTime.UtcNow.Year}", TextSyndicationContentKind.Html);
            feed.Description = new TextSyndicationContent(BlogConstant.BlogDescription);
            feed.Generator = "ASP.NET CORE LATEST";

            var items = new List<SyndicationItem>();
            var posts = _blogService.GetPosts().Where(p => p.IsPublic).OrderByDescending(p => p.CreationTimeUtc)
                .ToList();
            foreach (var blogPost in posts)
            {
                var textContent = new TextSyndicationContent(blogPost.HtmlContent);
                var item = new SyndicationItem(TitleHelper.GetTitle(blogPost.Title), textContent, new Uri(blogPost.FullUrl), blogPost.FullUrl, new DateTimeOffset(blogPost.LastUpdateTimeUtc, TimeSpan.Zero) );
                items.Add(item);
            }

            feed.Items = items;
            feed.Language = "zh-cn";
            return feed;
        }
    }
}
