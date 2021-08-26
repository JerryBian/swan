using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share.Site.Read;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    [Route("read")]
    public class ReadController : Controller
    {
        private readonly ApiSiteHttpClient _apiSiteHttpClient;

        public ReadController(ApiSiteHttpClient apiSiteHttpClient)
        {
            _apiSiteHttpClient = apiSiteHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _apiSiteHttpClient.GetReadItemsAsync();
            return View(model);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<BookItem> Get([FromRoute] string id)
        {
            return await _apiSiteHttpClient.GetReadItemAsync(id);
        }

        [HttpGet("add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] BookItem bookItem)
        {
            bookItem.IsCompleted = Request.Form["isCompleted"] == "on";
            await _apiSiteHttpClient.AddBookItemAsync(bookItem);
            return Redirect("/read");
        }

        [HttpGet("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] string id)
        {
            var bookItem = await _apiSiteHttpClient.GetReadItemAsync(id);
            return View(bookItem);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromForm] BookItem bookItem)
        {
            bookItem.IsCompleted = Request.Form["isCompleted"] == "on";
            await _apiSiteHttpClient.UpdateBookItemAsync(bookItem);
            return Redirect("/read");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task Delete([FromRoute] string id)
        {
            await _apiSiteHttpClient.DeleteBookItemAsync(id);
        }
    }
}