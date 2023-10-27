using Microsoft.AspNetCore.Mvc;
using Swan.Core.Model;
using Swan.Core.Service;

namespace Swan.Web.Controllers
{
    public class ReadController : Controller
    {
        private readonly ISwanService _swanService;

        public ReadController(ISwanService swanService)
        {
            _swanService = swanService;
        }

        public async Task<IActionResult> Index()
        {
            var readItems = await _swanService.FindAsync<SwanRead>(Request.HttpContext);

            return !readItems.Any() ? NotFound() : View(readItems.GroupBy(x => x.CreatedAt.Year).OrderByDescending(x => x.Key));
        }
    }
}
