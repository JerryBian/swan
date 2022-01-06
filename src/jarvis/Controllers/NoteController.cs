using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Misc;
using Laobian.Share.Model.Jarvis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.Controllers;

[Route("note")]
public class NoteController : Controller
{
    private readonly ILogger<NoteController> _logger;
    private readonly INoteGrpcService _noteGrpcService;
    private readonly JarvisOptions _options;
    private readonly NoteGrpcRequest _request;

    public NoteController(ILogger<NoteController> logger, IOptions<JarvisOptions> option)
    {
        _logger = logger;
        _options = option.Value;
        _request = new NoteGrpcRequest();
        _noteGrpcService = GrpcClientHelper.CreateClient<INoteGrpcService>(option.Value.ApiLocalEndpoint);
    }


    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int page)
    {
        try
        {
            const int itemsPerPage = 1;
            var totalCountResponse = await _noteGrpcService.GetNotesCountAsync(_request);
            if (totalCountResponse.IsOk)
            {
                var viewModel = new PagedViewModel<NoteRuntime>(page, totalCountResponse.Count, itemsPerPage)
                    {Url = Request.Path};
                _request.Count = itemsPerPage;
                _request.ExtractRuntime = true;
                _request.Offset = (viewModel.CurrentPage - 1) * itemsPerPage;
                var response = await _noteGrpcService.GetNotesAsync(_request);
                if (response.IsOk)
                {
                    response.Notes ??= new List<NoteRuntime>();
                    foreach (var noteRuntime in response.Notes)
                    {
                        viewModel.Items.Add(noteRuntime);
                    }
                }

                ViewData["Title"] = "所有笔记";
                if (viewModel.CurrentPage > 1)
                {
                    ViewData["Title"] = ViewData["Title"] + $"：第{viewModel.CurrentPage}页";
                }

                return View(viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index page has error.");
        }

        return NotFound();
    }

    [HttpGet]
    [Route("{link}.html")]
    public async Task<IActionResult> Detail([FromRoute] string link)
    {
        try
        {
            _request.Id = link;
            _request.ExtractRuntime = true;
            var response = await _noteGrpcService.GetNoteAsync(_request);
            if (response.IsOk)
            {
                return View(response.NoteRuntime);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detail page has error.");
        }

        return NotFound();
    }

    [HttpGet("tag")]
    public async Task<IActionResult> Tags()
    {
        try
        {
            _request.ExtractRuntime = true;
            var response = await _noteGrpcService.GetNoteTagsAsync(_request);
            if (response.IsOk)
            {
                return View(response.Tags);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tags page has error.");
        }

        return NotFound();
    }

    [HttpGet("tag/{link}")]
    public async Task<IActionResult> TagDetail([FromRoute] string link)
    {
        try
        {
            _request.ExtractRuntime = true;
            _request.TagLink = link;
            var response = await _noteGrpcService.GetNoteTagByLinkAsync(_request);
            if (response.IsOk)
            {
                return View(response.TagRuntime);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tags page has error.");
        }

        return NotFound();
    }
}