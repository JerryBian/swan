using Laobian.Lib;
using Laobian.Lib.Option;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LaobianOption _option;

        public HomeController(IOptions<LaobianOption> option)
        {
            _option = option.Value;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "添加新的文章";
            return View();
        }

    }
}
