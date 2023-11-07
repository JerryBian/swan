using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Option;
using Swan.Core.Service;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Swan.Web.Controllers
{
    [OutputCache]
    [ResponseCache(CacheProfileName = "Default")]
    public class PostController : Controller
    {
        private readonly SwanOption _option;
        private readonly ISwanService _swanService;

        public PostController(ISwanService swanService, IOptions<SwanOption> option)
        {
            _swanService = swanService;
            _option = option.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/post/{link}.html")]
        public async Task<IActionResult> GetPost([FromRoute] string link)
        {
            var post = await _swanService.FindFirstOrDefaultAsync<SwanPost>(Request.HttpContext, x => StringHelper.EqualsIgoreCase(link, x.Link));
            return post == null ? NotFound() : View("Detail", post);
        }

        [HttpGet("/post/archive")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _swanService.FindAsync<SwanPost>(Request.HttpContext);

            return !posts.Any() ? NotFound() : View("Archive", posts.GroupBy(x => x.PublishDate.Year).OrderByDescending(x => x.Key));
        }

        [HttpGet("/post/series")]
        public async Task<IActionResult> Series()
        {
            var tags = await _swanService.FindAsync<PostSeries>(Request.HttpContext);
            return View(tags.OrderBy(x => x.BlogPosts.Count));
        }

        [HttpGet("/post/tag")]
        public async Task<IActionResult> Tag()
        {
            var tags = await _swanService.FindAsync<PostTag>(Request.HttpContext);
            return View(tags.OrderBy(x => x.BlogPosts.Count));
        }

        [Route("/post/rss")]
        public async Task<IActionResult> Rss()
        {
            SyndicationFeed feed = new(_option.Title, _option.Description,
                    new Uri($"{_option.BaseUrl}/post/rss"),
                    "swan", DateTimeOffset.UtcNow)
            {
                Copyright = new TextSyndicationContent(
                        $"&#x26;amp;#169; {DateTime.Now.Year} {_option.Title}")
            };
            feed.Authors.Add(new SyndicationPerson(_option.ContactEmail,
                _option.Title,
                _option.BaseUrl));
            feed.BaseUri = new Uri(_option.BaseUrl);
            feed.Language = "zh-cn";
            List<SyndicationItem> items = new();

            var posts = await _swanService.FindAsync<SwanPost>(false);
            foreach (var post in posts)
            {
                items.Add(new SyndicationItem(post.Title, post.HtmlContent,
                    new Uri($"{_option.BaseUrl}{post.GetFullLink()}"),
                    $"{_option.BaseUrl}{post.GetFullLink()}",
                    new DateTimeOffset(post.LastUpdatedAt, TimeSpan.FromHours(8))));
            }

            feed.Items = items;
            XmlWriterSettings settings = new()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = false,
                Async = true,
                Indent = true,
                CheckCharacters = false
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
