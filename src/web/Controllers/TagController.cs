using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Service;

namespace Swan.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly ISwanService _swanService;

        public TagController(ISwanService swanService)
        {
            _swanService = swanService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/tag/{link}")]
        public async Task<IActionResult> Get([FromRoute] string link)
        {
            var allTags = await _swanService.GetBlogTagsAsync();
            var tag = allTags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Link, link));
            if(tag == null)
            {
                return NotFound();
            }

            return View("Detail", tag);
        }
    }
}
