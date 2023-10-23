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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/read/{year}/{id}")]
        public async Task<IActionResult> GetReadItem([FromRoute] int year, [FromRoute] string id)
        {
            var readItem = await _swanService.FindAsync<SwanRead>(id);
            return readItem == null || readItem.CreatedAt.Year != year ? NotFound() : View("Detail", readItem);
        }

        [HttpGet("/read/{year}")]
        public async Task<IActionResult> GetReadItems([FromRoute] int year)
        {
            var readItems = await _swanService.FindAsync<SwanRead>(x => x.CreatedAt.Year == year);
            return View("Archive", readItems);
        }
    }
}
