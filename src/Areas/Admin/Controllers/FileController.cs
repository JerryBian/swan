﻿using Laobian.Areas.Admin.Models;
using Laobian.Lib.Helper;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Areas.Admin.Controllers
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

        [HttpPost("/image/upload")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<IActionResult> Upload([FromForm(Name = "file")] IFormFile file)
        {
            StackEditorImageUploadRes res = new();
            try
            {
                string fileName = StringHelper.Random();
                string ext = Path.GetExtension(file.FileName);
                await using MemoryStream ms = new();
                await file.CopyToAsync(ms);
                _ = ms.Seek(0, SeekOrigin.Begin);
                string url = await _fileService.AddAsync(fileName + ext, ms.ToArray());
                res.UploadedImage = url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload failed.");
            }

            return Json(res);
        }
    }
}