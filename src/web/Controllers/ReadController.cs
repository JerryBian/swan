using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
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
            var readItems = await _swanService.GetReadItemsAsync();
            var readItem = readItems.FirstOrDefault(x => StringHelper.EqualsIgoreCase(id, x.Id));
            if (readItem == null || readItem.CreatedAt.Year != year)
            {
                return NotFound();
            }

            return View("Detail", readItem);
        }

        [HttpGet("/read/{year}")]
        public async Task<IActionResult> GetReadItems([FromRoute] int year)
        {
            var readItems = await _swanService.GetReadItemsAsync();
            readItems = readItems.Where(x => x.CreatedAt.Year == year).ToList();
            return View("Archive", readItems);
        }
    }
}
