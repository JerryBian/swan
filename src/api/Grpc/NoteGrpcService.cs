using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc;

public class NoteGrpcService : INoteGrpcService
{
    private readonly ILogger<NoteGrpcService> _logger;
    private readonly INoteFileService _noteFileService;

    public NoteGrpcService(ILogger<NoteGrpcService> logger, INoteFileService noteFileService)
    {
        _logger = logger;
        _noteFileService = noteFileService;
    }

    public async Task<NoteGrpcResponse> GetStatNotesPerYearAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var notes = await _noteFileService.GetNotesAsync();
            foreach (var item in notes.GroupBy(x => x.CreateTime.Year).OrderBy(x => x.Key))
            {
                response.Data.Add($"{item.Key}年", item.Count());
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetStatNotesPerYearAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetStatWordsPerYearAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var notes = await _noteFileService.GetNotesAsync();
            foreach (var item in notes.GroupBy(x => x.CreateTime.Year).OrderBy(x => x.Key))
            {
                response.Data.Add($"{item.Key}年", item.Sum(x => x.MdContent.Length));
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetStatWordsPerYearAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNotesAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var notes = await _noteFileService.GetNotesAsync(request.Year);
            if (request.Offset.HasValue)
            {
                notes = notes.Skip(request.Offset.Value).Take(request.Count).ToList();
            }

            var tags = await _noteFileService.GetNoteTagsAsync();
            foreach (var note in notes)
            {
                response.Notes.Add(GetNoteRuntime(note, request.ExtractRuntime, tags));
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNotesAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNotesCountAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var notes = await _noteFileService.GetNotesAsync(request.Year);
            response.Count = notes.Count;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNotesCountAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var note = await _noteFileService.GetNoteAsync(request.Id);
            if (note == null)
            {
                response.IsOk = false;
                response.Message = $"Note not found: {request.Id}";
            }
            else
            {
                var tags = await _noteFileService.GetNoteTagsAsync();
                response.NoteRuntime = GetNoteRuntime(note, request.ExtractRuntime, tags);
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNoteAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> AddNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            await _noteFileService.AddNoteAsync(request.Note);
            response.Note = request.Note;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(AddNoteAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> UpdateNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            await _noteFileService.UpdateNoteAsync(request.Note);
            response.Note = request.Note;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(UpdateNoteAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> DeleteNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            await _noteFileService.DeleteNoteAsync(request.Id);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(DeleteNoteAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetStatNotesPerTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var notes = await _noteFileService.GetNotesAsync();
            var tags = await _noteFileService.GetNoteTagsAsync();
            foreach (var tag in tags)
            {
                var runtime = GetNoteTagRuntime(tag, true, notes);
                response.Data.Add(tag.DisplayName, runtime.Notes.Count);
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetStatNotesPerTagAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteTagsAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var tags = await _noteFileService.GetNoteTagsAsync();
            foreach (var tag in tags)
            {
                var notes = await _noteFileService.GetNotesAsync();
                var runtime = GetNoteTagRuntime(tag, true, notes);
                response.Tags.Add(runtime);
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNoteTagsAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteTagByIdAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var tag = await _noteFileService.GetNoteTagByIdAsync(request.Id);
            if (tag == null)
            {
                response.IsOk = false;
                response.Message = $"Tag not found: {request.Id}";
            }
            else
            {
                var notes = await _noteFileService.GetNotesAsync();
                response.TagRuntime = GetNoteTagRuntime(tag, request.ExtractRuntime, notes);
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNoteTagByIdAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteTagByLinkAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            var tag = await _noteFileService.GetNoteTagByLinkAsync(request.TagLink);
            if (tag == null)
            {
                response.IsOk = false;
                response.Message = $"Tag not found: {request.TagLink}";
            }
            else
            {
                var notes = await _noteFileService.GetNotesAsync();
                response.TagRuntime = GetNoteTagRuntime(tag, request.ExtractRuntime, notes);
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNoteTagByLinkAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> AddNoteTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            await _noteFileService.AddNoteTagAsync(request.Tag);
            response.Tag = request.Tag;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(AddNoteTagAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> UpdateNoteTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            await _noteFileService.UpdateNoteTagAsync(request.Tag);
            response.Tag = request.Tag;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(UpdateNoteTagAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> DeleteNoteTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {
            await _noteFileService.DeleteNoteTagAsync(request.Id);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(DeleteNoteTagAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    private NoteRuntime GetNoteRuntime(Note note, bool extractRuntime, List<NoteTag> tags)
    {
        var runtime = new NoteRuntime(note);
        if (extractRuntime)
        {
            runtime.ExtractRuntimeData(tags);
        }

        return runtime;
    }

    private NoteTagRuntime GetNoteTagRuntime(NoteTag tag, bool extractRuntime, List<Note> notes)
    {
        var runtime = new NoteTagRuntime(tag);
        if (extractRuntime)
        {
            runtime.ExtractRuntime(notes);
        }

        return runtime;
    }
}