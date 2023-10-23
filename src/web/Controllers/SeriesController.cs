using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Service;

namespace Swan.Web.Controllers
{
    public class SeriesController : Controller
    {
        private readonly ISwanService _swanService;

        public SeriesController(ISwanService swanService)
        {
            _swanService = swanService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/series/{link}")]
        public async Task<IActionResult> Get([FromRoute] string link)
        {
            var series = await _swanService.FindAsync<SwanSeries>(x => StringHelper.EqualsIgoreCase(x.Link, link));
            return series == null ? NotFound() : View("Detail", series);
        }
    }
}
