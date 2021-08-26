using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Site.Read;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("read")]
    public class ReadApiController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;

        public ReadApiController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookItem>>> GetAll()
        {
            var readItems = await _fileRepository.GetBookItemsAsync();
            return Ok(readItems);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<BookItem>> Get([FromRoute] string id)
        {
            var readItems = await _fileRepository.GetBookItemsAsync();
            var result = readItems.FirstOrDefault(x => x.Id == id);
            if (result == null)
            {
                return NotFound($"Book item with id \"{id}\" does not exist.");
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Add(BookItem bookItem)
        {
            await _fileRepository.AddBookItemAsync(bookItem);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Update(BookItem bookItem)
        {
            await _fileRepository.UpdateBookItemAsync(bookItem);
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            await _fileRepository.DeleteBookItemAsync(id);
            return Ok();
        }
    }
}