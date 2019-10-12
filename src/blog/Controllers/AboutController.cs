using Laobian.Share.BlogEngine;
using Laobian.Share.BlogEngine.Model;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class AboutController : Controller
    {
        private readonly IBlogService _blogService;

        public AboutController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [ResponseCache(VaryByHeader = "Accept-Encoding", Duration = 60 * 60 * 24, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            var html = _blogService.GetAboutHtml(RequestLang.English);

            ViewData["Title"] = "关于";
            ViewData["Canonical"] = "/about/";
            ViewData["Description"] = "关于作者以及这个博客的一切";
            return View(model: html);
        }
    }
}
