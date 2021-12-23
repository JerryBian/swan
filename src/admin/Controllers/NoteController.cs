using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Admin.Models;
using Laobian.Share;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("note")]
public class NoteController : Controller
{
    private readonly INoteGrpcService _noteGrpcService;
    private readonly AdminOptions _options;
    private readonly ILogger<NoteController> _logger;
    private readonly NoteGrpcRequest _request;

    public NoteController(IOptions<AdminOptions> option, ILogger<NoteController> logger)
    {
        _logger = logger;
        _options = option.Value;
        _request = new NoteGrpcRequest();
        _noteGrpcService = GrpcClientHelper.CreateClient<INoteGrpcService>(_options.ApiLocalEndpoint);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("stats/notes-per-year")]
    public async Task<ApiResponse<ChartResponse>> GetNotesPerYearChart()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var noteResponse = await _noteGrpcService.GetStatNotesPerYearAsync(_request);
            if (noteResponse.IsOk)
            {
                noteResponse.Data ??= new Dictionary<string, int>();
                var chartResponse = new ChartResponse
                {
                    Title = "当年文章的总数",
                    Type = "line"
                };
                foreach (var item in noteResponse.Data)
                {
                    chartResponse.Data.Add(item.Value);
                    chartResponse.Labels.Add(item.Key);
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = noteResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNotesPerYearChart)} failed.");
        }

        return response;
    }

    [HttpPost("stats/words-per-year")]
    public async Task<ApiResponse<ChartResponse>> GetWordsPerYearChart()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var noteResponse = await _noteGrpcService.GetStatWordsPerYearAsync(_request);
            if (noteResponse.IsOk)
            {
                noteResponse.Data ??= new Dictionary<string, int>();
                var chartResponse = new ChartResponse
                {
                    Title = "当年文章的总字数",
                    Type = "line"
                };
                foreach (var item in noteResponse.Data)
                {
                    chartResponse.Data.Add(item.Value);
                    chartResponse.Labels.Add(item.Key);
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = noteResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetWordsPerYearChart)} failed.");
        }

        return response;
    }

    [HttpPost("stats/notes-per-tag")]
    public async Task<ApiResponse<ChartResponse>> GetNotesPerTagChart()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var noteResponse = await _noteGrpcService.GetStatNotesPerTagAsync(_request);
            if (noteResponse.IsOk)
            {
                noteResponse.Data ??= new Dictionary<string, int>();
                var chartResponse = new ChartResponse
                {
                    Title = "当前标签的笔记数",
                    Type = "line"
                };
                foreach (var item in noteResponse.Data)
                {
                    chartResponse.Data.Add(item.Value);
                    chartResponse.Labels.Add(item.Key);
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = noteResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNotesPerTagChart)} failed.");
        }

        return response;
    }

    [HttpGet]
    [Route("post/add")]
    public async Task<IActionResult> AddNote()
    {
        var noteResponse = await _noteGrpcService.GetNoteTagsAsync(_request);
        if (noteResponse.IsOk)
        {
            return View("AddNote", noteResponse.Tags ?? new List<NoteTagRuntime>());
        }

        return NotFound(noteResponse.Message);
    }

    [HttpPost("post")]
    public async Task<ApiResponse<object>> AddPost([FromForm] Note note)
    {
        var response = new ApiResponse<object>();
        try
        {
            _request.Note = note;
            var noteResponse = await _noteGrpcService.AddNoteAsync(_request);
            if (noteResponse.IsOk)
            {
                response.RedirectTo = noteResponse.Note.GetFullPath(_options);
            }
            else
            {
                response.IsOk = false;
                response.Message = noteResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add note failed. {Environment.NewLine}{JsonUtil.Serialize(note)}");
        }

        return response;
    }

    [HttpGet("post/{id}/update")]
    public async Task<IActionResult> UpdateNote([FromRoute] string id)
    {
        _request.Id = id;
        var noteResponse = await _noteGrpcService.GetNoteAsync(_request);
        if (noteResponse.IsOk)
        {
            var model = new NotePostUpdateViewModel() { Post = noteResponse.NoteRuntime.Raw };
            noteResponse = await _noteGrpcService.GetNoteTagsAsync(_request);
            if (noteResponse.IsOk)
            {
                model.Tags.AddRange(noteResponse.Tags.Select(x => x.Raw));
                return View("UpdateNote", model);
            }
        }

        return NotFound(noteResponse.Message);
    }

    [HttpPut("post")]
    public async Task<ApiResponse<object>> UpdatePost([FromForm] Note note)
    {
        var response = new ApiResponse<object>();
        try
        {
            _request.Note = note;
            var noteResponse = await _noteGrpcService.UpdateNoteAsync(_request);
            if (noteResponse.IsOk)
            {
                response.RedirectTo = noteResponse.Note.GetFullPath(_options);
            }
            else
            {
                response.IsOk = false;
                response.Message = noteResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update note failed. {Environment.NewLine}{JsonUtil.Serialize(note)}");
        }

        return response;
    }

    [Route("tag")]
    public async Task<IActionResult> GetTagsAsync()
    {
        var noteResponse = await _noteGrpcService.GetNoteTagsAsync(_request);
        if (noteResponse.IsOk)
        {
            noteResponse.Tags ??= new List<NoteTagRuntime>();
            return View("Tags", noteResponse.Tags.Select(x => x.Raw));
        }

        return NotFound(noteResponse.Message);
    }

    [HttpDelete]
    [Route("tag/{id}")]
    public async Task<ApiResponse<object>> DeleteTagAsync([FromRoute] string id)
    {
        var response = new ApiResponse<object>();
        try
        {
            _request.Id = id;
            await _noteGrpcService.DeleteNoteTagAsync(_request);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Delete note tag {id} failed.");
        }

        return response;
    }

    [HttpGet("tag/add")]
    public IActionResult AddTag()
    {
        return View("AddTag");
    }

    [HttpPost]
    [Route("tag")]
    public async Task<ApiResponse<object>> AddTagAsync([FromForm] NoteTag tag)
    {
        var response = new ApiResponse<object>();
        try
        {
            _request.Tag = tag;
            var blogResponse = await _noteGrpcService.AddNoteTagAsync(_request);
            if (blogResponse.IsOk)
            {
                response.RedirectTo = "/note/tag";
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add note tag {JsonUtil.Serialize(tag)} failed.");
        }

        return response;
    }

    [HttpGet("tag/{id}/update")]
    public async Task<IActionResult> UpdateTagAsync([FromRoute] string id)
    {
        _request.Id = id;
        var blogResponse = await _noteGrpcService.GetNoteTagByIdAsync(_request);
        if (blogResponse.IsOk)
        {
            return View("UpdateTag", blogResponse.TagRuntime.Raw);
        }

        return NotFound(blogResponse.Message);
    }

    [HttpPut]
    [Route("tag")]
    public async Task<ApiResponse<object>> UpdateTagAsync([FromForm] NoteTag tag)
    {
        var response = new ApiResponse<object>();
        try
        {
            _request.Tag = tag;
            var blogResponse = await _noteGrpcService.UpdateNoteTagAsync(_request);
            if (blogResponse.IsOk)
            {
                response.RedirectTo = "/note/tag";
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update tag failed. {JsonUtil.Serialize(tag)}");
        }

        return response;
    }
}