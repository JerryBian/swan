using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpService;
using Laobian.Share.Read;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    [Route("read")]
    public class ReadController : Controller
    {
        private readonly ApiHttpService _apiHttpService;

        public ReadController(ApiHttpService apiHttpService)
        {
            _apiHttpService = apiHttpService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _apiHttpService.GetReadItemsAsync();
            return View(model);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ReadItem> Get([FromRoute]string id)
        {
            return await _apiHttpService.GetReadItemAsync(id);
        }

        [HttpPut]
        public async Task Add([FromBody]ReadItem readItem)
        {
            await _apiHttpService.AddReadItemAsync(readItem);
        }

        [HttpPost]
        public async Task Update([FromBody]ReadItem readItem)
        {
            await _apiHttpService.UpdateReadItemAsync(readItem);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task Delete([FromRoute]string id)
        {
            await _apiHttpService.DeleteReadItemAsync(id);
        }
    }
}
