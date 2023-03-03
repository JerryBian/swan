using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swan.Core;
using Swan.Core.Extension;
using Swan.Core.Option;
using Swan.Core.Service;
using System.Text;

namespace Swan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly SwanOption _option;

        public HomeController(IBlogService blogService, IOptions<SwanOption> option)
        {
            _option = option.Value;
            _blogService = blogService;
        }

        [ResponseCache(CacheProfileName = Constants.Misc.CacheProfileServerLong)]
        public IActionResult Index()
        {
            ViewData[Constants.ViewData.Description] = $"{_option.Description}";
            return View();
        }

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        [ResponseCache(CacheProfileName = Constants.Misc.CacheProfileServerLong)]
        public async Task<IActionResult> Sitemap()
        {
            StringBuilder sb = new();
            _ = sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            _ = sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/read</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");

            List<Core.Model.BlogPost> posts = await _blogService.GetAllPostsAsync(false);
            foreach (Core.Model.BlogPost post in posts)
            {
                _ = sb.AppendLine(
                    $"<url><loc>{_option.BaseUrl}{post.GetUrl()}</loc><lastmod>{post.Object.LastUpdateTime.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.6</priority></url>");
            }

            _ = sb.AppendLine("</urlset>");
            string sitemap = sb.ToString();
            return Content(sitemap, "text/xml", Encoding.UTF8);
        }
    }
}