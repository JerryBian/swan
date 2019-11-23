using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICacheClient _cacheClient;

        public SubscribeController(ICacheClient cacheClient, IBlogService blogService)
        {
            _blogService = blogService;
            _cacheClient = cacheClient;
        }

        [Route("/rss")]
        public IActionResult Rss()
        {
            var rss = _cacheClient.GetOrCreate(CacheKey.Build(nameof(SubscribeController), nameof(Rss)), () =>
            {
                var feed = GetFeed();
                var rssFormatter = new Rss20FeedFormatter(feed) {SerializeExtensionsAsAtom = false};
                using (var ms = new MemoryStream())
                {
                    using (var xmlWriter = XmlWriter.Create(ms,
                        new XmlWriterSettings {Async = true, Encoding = Encoding.UTF8}))
                    {
                        rssFormatter.WriteTo(xmlWriter);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }, new BlogAssetChangeToken());

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
                    using (var xmlWriter = XmlWriter.Create(ms,
                        new XmlWriterSettings {Async = true, Encoding = Encoding.UTF8}))
                    {
                        atomFormatter.WriteTo(xmlWriter);
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }, new BlogAssetChangeToken());

            return Content(atom, "application/atom+xml", Encoding.UTF8);
        }

        private SyndicationFeed GetFeed()
        {
            var feed = new SyndicationFeed("", "", new Uri(Global.Config.Blog.BlogAddress))
            {
                Title = new TextSyndicationContent(Global.Config.Blog.Name, TextSyndicationContentKind.Plaintext),
                Copyright = new TextSyndicationContent(
                    $"Copyright {DateTime.UtcNow.Year} {Global.Config.Common.AdminChineseName}",
                    TextSyndicationContentKind.Plaintext),
                Description =
                    new TextSyndicationContent(Global.Config.Blog.Description, TextSyndicationContentKind.Plaintext),
                Generator = "ASP.NET CORE",
                BaseUri = new Uri(Global.Config.Blog.BlogAddress),
                Id = Global.Config.Blog.BlogAddress,
                Language = "zh-cn",
                LastUpdatedTime = DateTimeOffset.UtcNow,
                TimeToLive = TimeSpan.FromHours(1)
            };

            var sp = new SyndicationPerson(Global.Config.Common.AdminEmail, Global.Config.Common.AdminChineseName,
                Global.Config.Blog.BlogAddress);
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