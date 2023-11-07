using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Model;
using Swan.Core.Option;
using Swan.Core.Service;
using System.Text;

namespace Swan.Web.Controllers
{
    [OutputCache]
    [ResponseCache(CacheProfileName = "Default")]
    public class HomeController : Controller
    {
        private readonly SwanOption _option;
        private readonly ISwanService _swanService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IOptions<SwanOption> option, ISwanService swanService)
        {
            _logger = logger;
            _option = option.Value;
            _swanService = swanService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            StringBuilder sb = new();
            _ = sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            _ = sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/post</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/read</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/post/archive</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.8</priority></url>");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/post/tag</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.7</priority></url>");
            _ = sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/post/series</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.7</priority></url>");

            var posts = await _swanService.FindAsync<SwanPost>(false);
            foreach (var post in posts)
            {
                _ = sb.AppendLine(
                    $"<url><loc>{_option.BaseUrl}{post.GetFullLink()}</loc><lastmod>{post.LastUpdatedAt.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.6</priority></url>");
            }

            _ = sb.AppendLine("</urlset>");
            string sitemap = sb.ToString();
            return Content(sitemap, "text/xml", Encoding.UTF8);
        }
    }
}