using Microsoft.AspNetCore.Mvc;

namespace Swan.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
