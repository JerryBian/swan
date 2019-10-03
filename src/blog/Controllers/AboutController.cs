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

        public IActionResult Index()
        {
            var html = _blogService.GetAboutHtml(RequestLang.English);

            ViewData["Title"] = "关于";
            return View(model: html);
        }
    }
}
