using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share.Read;
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
        public async Task<BookItem> Get([FromRoute]string id)
        {
            return await _apiSiteHttpClient.GetReadItemAsync(id);
        }

        [HttpPut]
        public async Task Add([FromBody]BookItem bookItem)
        {
            await _apiSiteHttpClient.AddReadItemAsync(bookItem);
        }

        [HttpPost]
        public async Task Update([FromBody]BookItem bookItem)
        {
            await _apiSiteHttpClient.UpdateReadItemAsync(bookItem);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task Delete([FromRoute]string id)
        {
            await _apiSiteHttpClient.DeleteReadItemAsync(id);
        }
    }
}
