using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "添加新的文章";
            return View();
        }

        [HttpPost("/admin/file/upload")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> Upload([FromForm(Name = "file")]IFormFile file)
        {
            ApiResponse<string> res = new();
            try
            {
                var fileName = StringHelper.Random();
                var ext = Path.GetExtension(file.FileName);
                await using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var url = await _fileService.AddAsync(fileName + ext, ms.ToArray());
                res.Content = url;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.IsOk = false;
                _logger.LogError(ex, "File upload failed.");
            }

            return Json(res);
        }
    }
}
