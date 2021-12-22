using System;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

    public async Task<NoteGrpcResponse> GetNoteStatCountPerYearAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"{nameof(GetNoteStatCountPerYearAsync)} failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteStatWordsPerYearAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNotesAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> AddNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> UpdateNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> DeleteNoteAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteTagStatCountPerYearAsync(NoteGrpcRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public async Task<NoteGrpcResponse> GetNoteTagsAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteTagByIdAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> GetNoteTagByLinkAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> AddNoteTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> UpdateNoteTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }

    public async Task<NoteGrpcResponse> DeleteNoteTagAsync(NoteGrpcRequest request, CallContext context = default)
    {
        var response = new NoteGrpcResponse();
        try
        {

        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get notes failed. {JsonUtil.Serialize(request)}");
        }

        return response;
    }
}