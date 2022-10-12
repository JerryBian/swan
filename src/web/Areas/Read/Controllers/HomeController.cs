using Laobian.Lib;
using Laobian.Lib.Service;
using Laobian.Web.Areas.Read.Models;
using Microsoft.AspNetCore.Mvc;

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

        [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            List<Lib.Model.ReadItemView> items = await _readService.GetAllAsync();
            if (!isAuthenticated)
            {
                items = items.Where(x => x.Raw.IsPublic).ToList();
            }

            var model = new List<ReadIndexViewModel>();
            foreach(var item in items.GroupBy(x => x.Raw.CreateTime.Year).OrderByDescending(x => x.Key))
            {
                var vm = new ReadIndexViewModel
                {
                    Title = item.Key.ToString(),
                    Id = item.Key.ToString(),
                    Count = item.Count(),
                    Items = item.OrderByDescending(x => x.Raw.CreateTime).ToList()
                };
                model.Add(vm);
            }

            ViewData["Title"] = $"阅读";
            ViewData["DatePublished"] = items.Min(x => x.Raw.CreateTime);
            ViewData["DateModified"] = items.Max(x => x.Raw.LastUpdateTime);
            return View(model);
        }
    }
}
