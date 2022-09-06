using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Laobian.Web.Areas.Read.Controllers
{
    [Area(Constants.AreaRead)]
    public class HomeController : Controller
    {
        private readonly IReadService _readService;

        public HomeController(IReadService readService)
        {
            _readService = readService;
        }

        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            var items = await _readService.GetAllAsync();
            if (!isAuthenticated)
            {
                items = items.Where(x => x.Raw.IsPublic).ToList();
            }

            ViewData["Title"] = $"阅读";
            ViewData["DatePublished"] = items.Min(x => x.Raw.CreateTime);
            ViewData["DateModified"] = items.Max(x => x.Raw.LastUpdateTime);
            return View(items);
        }
    }
}
