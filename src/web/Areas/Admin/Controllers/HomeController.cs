using Laobian.Lib;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
