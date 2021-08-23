using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class ReadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}