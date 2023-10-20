using Microsoft.AspNetCore.Mvc;

namespace Swan.Web.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
