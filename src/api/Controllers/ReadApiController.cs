using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Laobian.Share.Read;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("read")]
    public class ReadApiController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public ReadApiController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<ActionResult<IDictionary<int, List<ReadItem>>>> GetAll()
        {
            var readItems = await _blogService.GetReadItemsAsync();
            return Ok(readItems);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ReadItem>> Get([FromRoute]string id)
        {
            var readItem = await _blogService.GetReadItemAsync(id);
            return Ok(readItem);
        }

        [HttpPut]
        public async Task<IActionResult> Add(ReadItem readItem)
        {
            await _blogService.AddReadItemAsync(readItem);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ReadItem readItem)
        {
            await _blogService.UpdateReadItemAsync(readItem);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]string id)
        {
            await _blogService.RemoveReadItemAsync(id);
            return Ok();
        }
    }
}
