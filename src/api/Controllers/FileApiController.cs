using System.IO;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("file")]
    public class FileApiController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;

        public FileApiController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> Upload([FromQuery] string fileName)
        {
            await using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var url = await _fileRepository.AddRawFileAsync(fileName, ms.ToArray());
            return Ok(url);
        }
    }
}