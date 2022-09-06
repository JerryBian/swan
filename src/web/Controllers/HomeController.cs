using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Extension;
using Laobian.Lib.Option;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace Laobian.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly LaobianOption _option;

        public HomeController(IBlogService blogService, IOptions<LaobianOption> option)
        {
            _option = option.Value;
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        [Route("/sitemap")]
        [Route("/sitemap.xml")]
        [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
        public async Task<IActionResult> Sitemap()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>1.0</priority></url>");
            sb.AppendLine(
                $"<url><loc>{_option.BaseUrl}/read</loc><lastmod>{DateTime.Now.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.9</priority></url>");

            var posts = await _blogService.GetAllPostsAsync();
            foreach (var post in posts.Where(x => x.IsPublished()))
            {
                sb.AppendLine(
                    $"<url><loc>{_option.BaseUrl}{post.FullLink}</loc><lastmod>{post.Raw.LastUpdateTime.ToDate()}</lastmod><changefreq>daily</changefreq><priority>0.6</priority></url>");
            }

            sb.AppendLine("</urlset>");
            var sitemap = sb.ToString();
            return Content(sitemap, "text/xml", Encoding.UTF8);
        }
    }
}
