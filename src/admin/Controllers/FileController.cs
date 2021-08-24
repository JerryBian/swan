using System.IO;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    [Route("file")]
    public class FileController : Controller
    {
        private readonly ApiSiteHttpClient _httpClient;

        public FileController(ApiSiteHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<ActionResult<string>> Upload(IFormFile image)
        {
            var fileName = Path.GetRandomFileName().Replace(".", "");
            var ext = Path.GetExtension(image.FileName);
            await using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var url = await _httpClient.UploadFileAsync(fileName + ext, ms.ToArray());
            if (string.IsNullOrEmpty(url))
            {
                var errorObj = new {error = "Server Error."};
                return Json(errorObj);
            }

            var okObj = new {data = new {filePath = url}};
            return Json(okObj);
        }
    }
}