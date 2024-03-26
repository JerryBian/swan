using Microsoft.AspNetCore.Mvc;

namespace Swan.Controllers
{
    public class ReadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
