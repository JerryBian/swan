using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Log()
        {
            return View();
        }
    }
}
