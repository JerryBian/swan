using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
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
            var allSeries = await _swanService.GetBlogSeriesAsync();
            var series = allSeries.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Link, link));
            if(series == null)
            {
                return NotFound();
            }

            return View("Detail", series);
        }
    }
}
