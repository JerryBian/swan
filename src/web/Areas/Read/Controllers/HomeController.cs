using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Read.Controllers
{
    [Area(Constants.AreaRead)]
    public class HomeController : Controller
    {
        private readonly IReadService _readService;
        private readonly ICacheManager _cacheManager;

        public HomeController(IReadService readService, ICacheManager cacheManager)
        {
            _readService = readService;
            _cacheManager = cacheManager;
        }

        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            List<Lib.Model.ReadItemView> model = await _cacheManager.GetOrCreateAsync($"{Constants.AreaRead}_{nameof(HomeController)}_{nameof(Index)}_{isAuthenticated}", async () =>
            {
                List<Lib.Model.ReadItemView> items = await _readService.GetAllAsync();
                if (!isAuthenticated)
                {
                    items = items.Where(x => x.Raw.IsPublic).ToList();
                }

                return items;
            });

            return View(model);
        }
    }
}
