using System;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("file")]
public class FileController : Controller
{
    private readonly IFileGrpcService _fileGrpcService;
    private readonly ILogger<FileController> _logger;

    public FileController(IOptions<AdminOptions> options, ILogger<FileController> logger)
    {
        _logger = logger;
        _fileGrpcService = GrpcClientHelper.CreateClient<IFileGrpcService>(options.Value.ApiLocalEndpoint);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<ActionResult<string>> Upload(IFormFile image)
    {
        try
        {
            var fileName = StringUtil.GenerateRandom();
            var ext = Path.GetExtension(image.FileName);
            await using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var request = new FileGrpcRequest {Content = ms.ToArray(), FileName = fileName + ext};
            var response = await _fileGrpcService.AddFileAsync(request);
            if (response.IsOk)
            {
                var okObj = new {data = new {filePath = response.Url}};
                return Json(okObj);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed.");
        }

        var errorObj = new {error = "Server Error."};
        return Json(errorObj);
    }
}