using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Areas.Admin.Controllers
{
    [Authorize]
    [Area("admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/admin/log");
        }
    }
}