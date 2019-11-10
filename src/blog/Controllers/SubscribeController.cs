using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Laobian.Share.Blog;
using Laobian.Share.Cache;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly ICacheClient _cacheClient;
        private readonly IBlogService _blogService;

        public SubscribeController(IOptions<AppConfig> appConfig, ICacheClient cacheClient, IBlogService blogService)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
            _cacheClient = cacheClient;
        }

        [Route("/rss")]
        public IActionResult Rss()
        {
            var rss = _cacheClient.GetOrCreate(CacheKey.Build(nameof(SubscribeController), nameof(Rss)), () =>
            {
                var feed = GetFeed();
                var rssFormatter = new Rss20FeedFormatter(feed) { SerializeExtensionsAsAtom = false };
                using (var ms = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8 }))
                    {
                        rssFormatter.WriteTo(xmlWriter);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            });

            return Content(rss, "application/rss+xml", Encoding.UTF8);
        }

        [Route("/atom")]
        public IActionResult Atom()
        {
            var atom = _cacheClient.GetOrCreate(CacheKey.Build(nameof(SubscribeController), nameof(Atom)), () =>
            {
                var feed = GetFeed();
                var atomFormatter = new Atom10FeedFormatter(feed);
                using (var ms = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8 }))
                    {
                        atomFormatter.WriteTo(xmlWriter);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            });

            return Content(atom, "application/atom+xml", Encoding.UTF8);
        }

        private SyndicationFeed GetFeed()
        {
            var feed = new SyndicationFeed("", "", new Uri(_appConfig.Blog.BlogAddress))
            {
                Title = new TextSyndicationContent(_appConfig.Blog.Name, TextSyndicationContentKind.Plaintext),
                Copyright = new TextSyndicationContent(
                    $"Copyright {DateTime.UtcNow.Year} {_appConfig.Common.AdminChineseName}",
                    TextSyndicationContentKind.Plaintext),
                Description =
                    new TextSyndicationContent(_appConfig.Blog.Description, TextSyndicationContentKind.Plaintext),
                Generator = "ASP.NET CORE",
                BaseUri = new Uri(_appConfig.Blog.BlogAddress),
                Id = _appConfig.Blog.BlogAddress,
                Language = "zh-cn",
                LastUpdatedTime = DateTimeOffset.UtcNow,
                TimeToLive = TimeSpan.FromHours(1)
            };

            var sp = new SyndicationPerson(_appConfig.Common.AdminEmail, _appConfig.Common.AdminChineseName, _appConfig.Blog.BlogAddress);
            feed.Authors.Add(sp);
            feed.Contributors.Add(sp);
            
            var items = new List<SyndicationItem>();
            var posts = _blogService.GetPosts();
            foreach (var blogPost in posts)
            {
                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent(blogPost.Title, TextSyndicationContentKind.Plaintext),
                    Copyright = feed.Copyright,
                    Id = blogPost.FullUrlWithBase,
                    PublishDate = new DateTimeOffset(blogPost.PublishTime, TimeSpan.FromHours(8)),
                    Summary = new TextSyndicationContent(blogPost.ExcerptHtml, TextSyndicationContentKind.Html),
                    Content = new TextSyndicationContent(blogPost.ContentHtml, TextSyndicationContentKind.Html),
                    LastUpdatedTime = new DateTimeOffset(blogPost.LastUpdateTime, TimeSpan.FromHours(8))
                };

                item.AddPermalink(new Uri(blogPost.FullUrlWithBase));
                item.Authors.Add(sp);
                item.Contributors.Add(sp);

                foreach (var cat in blogPost.Categories)
                {
                    item.Categories.Add(new SyndicationCategory(cat.Name));
                }

                items.Add(item);
            }

            feed.Items = items;
            
            return feed;
        }
    }
}
