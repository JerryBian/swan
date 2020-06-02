using Laobian.Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Areas.Admin.Controllers
{
    [Authorize]
    [Area("admin")]
    public class LogController : Controller
    {
        public IActionResult Index()
        {
            ViewData[ViewDataConstant.Title] = "View Logs";
            ViewData[ViewDataConstant.VisibleToSearchEngine] = false;
            return View();
        }
    }
}
