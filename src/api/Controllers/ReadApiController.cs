using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Site.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Api.Controllers;

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
    public async Task<ActionResult<List<ReadItem>>> GetAll()
    {
        var readItems = await _fileRepository.GetReadItemsAsync();
        return Ok(readItems);
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ReadItem>> Get([FromRoute] string id)
    {
        var readItems = await _fileRepository.GetReadItemsAsync();
        var result = readItems.FirstOrDefault(x => x.Id == id);
        if (result == null)
        {
            return NotFound($"Book item with id \"{id}\" does not exist.");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReadItem>> Add(ReadItem readItem)
    {
        await _fileRepository.AddReadItemAsync(readItem);
        return Ok(readItem);
    }

    [HttpPut]
    public async Task<ActionResult<ReadItem>> Update(ReadItem readItem)
    {
        await _fileRepository.UpdateReadItemAsync(readItem);
        return Ok(readItem);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await _fileRepository.DeleteReadItemAsync(id);
        return Ok();
    }
}