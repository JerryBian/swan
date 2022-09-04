using Laobian.Lib;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    public class HomeController : Controller
    {
        private readonly LaobianOption _option;

        public HomeController(IOptions<LaobianOption> option)
        {
            _option = option.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
