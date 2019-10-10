using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public SubscribeController(IOptions<AppConfig> appConfig, IBlogService blogService)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        [Route("/rss")]
        public IActionResult Rss()
        {
            var feed = GetFeed();
            var rssFormatter = new Rss20FeedFormatter(feed) { SerializeExtensionsAsAtom = false };
            using (var ms = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8 }))
                {
                    rssFormatter.WriteTo(xmlWriter);
                }

                return Content(Encoding.UTF8.GetString(ms.ToArray()), "application/rss+xml", Encoding.UTF8);
            }
        }

        [Route("/atom")]
        public IActionResult Atom()
        {
            var feed = GetFeed();
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

        private SyndicationFeed GetFeed()
        {
            var feed = new SyndicationFeed("", "", new Uri(_appConfig.BlogAddress))
            {
                Title = new TextSyndicationContent(BlogConstant.BlogName, TextSyndicationContentKind.Plaintext),
                Copyright = new TextSyndicationContent(
                    $"Copyright {DateTime.UtcNow.Year} {BlogConstant.AuthorChineseName}",
                    TextSyndicationContentKind.Plaintext),
                Description =
                    new TextSyndicationContent(BlogConstant.BlogDescription, TextSyndicationContentKind.Plaintext),
                Generator = "ASP.NET CORE",
                BaseUri = new Uri(_appConfig.BlogAddress),
                Id = _appConfig.BlogAddress,
                Language = "zh-cn",
                LastUpdatedTime = DateTimeOffset.UtcNow,
                TimeToLive = TimeSpan.FromHours(1)
            };

            var sp = new SyndicationPerson(BlogConstant.AuthorEmail, BlogConstant.AuthorChineseName, _appConfig.BlogAddress);
            feed.Authors.Add(sp);
            feed.Contributors.Add(sp);
            
            var items = new List<SyndicationItem>();
            var posts = _blogService.GetPosts().Where(p => p.IsPublic).OrderByDescending(p => p.CreationTimeUtc)
                .ToList();
            foreach (var blogPost in posts)
            {
                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent(blogPost.Title, TextSyndicationContentKind.Plaintext),
                    Copyright = feed.Copyright,
                    Id = blogPost.FullUrlWithBaseAddress,
                    PublishDate = new DateTimeOffset(blogPost.CreationTimeUtc, TimeSpan.Zero),
                    Summary = new TextSyndicationContent(blogPost.Excerpt, TextSyndicationContentKind.Html),
                    Content = new TextSyndicationContent(blogPost.HtmlContent, TextSyndicationContentKind.Html),
                    LastUpdatedTime = new DateTimeOffset(blogPost.LastUpdateTimeUtc, TimeSpan.Zero)
                };

                item.AddPermalink(new Uri(blogPost.FullUrlWithBaseAddress));
                item.Authors.Add(sp);
                item.Contributors.Add(sp);

                foreach (var cat in blogPost.CategoryNames)
                {
                    item.Categories.Add(new SyndicationCategory(cat));
                }

                items.Add(item);
            }

            feed.Items = items;
            
            return feed;
        }
    }
}
