using System;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc;

public class FileGrpcService : IFileGrpcService
{
    private readonly ILogger<FileGrpcService> _logger;
    private readonly IRawFileService _rawFileService;

    public FileGrpcService(IRawFileService rawFileService, ILogger<FileGrpcService> logger)
    {
        _logger = logger;
        _rawFileService = rawFileService;
    }

    public async Task<FileGrpcResponse> AddFileAsync(FileGrpcRequest request, CallContext context = default)
    {
        var response = new FileGrpcResponse();
        try
        {
            response.Url = await _rawFileService.AddRawFileAsync(request.FileName, request.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Add file failed, {JsonUtil.Serialize(request)}");
            response.IsOk = false;
            response.Message = ex.Message;
        }

        return response;
    }
}