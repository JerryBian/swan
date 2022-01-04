using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Api.Service;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Logger;
using Laobian.Share.Model;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace Laobian.Api.Grpc;

public class LogGrpcService : ILogGrpcService
{
    private readonly ILaobianLogQueue _laobianLogQueue;
    private readonly ILogFileService _logFileService;
    private readonly ILogger<LogGrpcService> _logger;

    public LogGrpcService(ILogger<LogGrpcService> logger, ILaobianLogQueue laobianLogQueue,
        ILogFileService logFileService)
    {
        _logger = logger;
        _logFileService = logFileService;
        _laobianLogQueue = laobianLogQueue;
    }

    public async Task<LogGrpcResponse> AddLogsAsync(LogGrpcRequest request, CallContext context = default)
    {
        var response = new LogGrpcResponse();
        try
        {
            foreach (var log in request.Logs)
            {
                log.LoggerName = request.Logger;
                _laobianLogQueue.Add(log);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LogGrpcService)}({nameof(AddLogsAsync)}) failed.");
            response.IsOk = false;
            response.Message = ex.Message;
        }

        return await Task.FromResult(response);
    }

    public async Task<LogGrpcResponse> GetLogsAsync(LogGrpcRequest request, CallContext context = default)
    {
        var response = new LogGrpcResponse();
        try
        {
            var logs = new List<LaobianLog>();
            var days = request.Days;
            var minLevel = request.MinLevel;
            if (Enum.TryParse(request.Logger, true, out LaobianSite laobianSite))
            {
                if (laobianSite == LaobianSite.All)
                {
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Admin, days, minLevel));
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Blog, days, minLevel));
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Api, days, minLevel));
                    logs.AddRange(await ReadLogsAsync(LaobianSite.Jarvis, days, minLevel));
                }
                else
                {
                    logs.AddRange(await ReadLogsAsync(laobianSite, days, minLevel));
                }
            }

            response.Logs = logs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LogGrpcService)}({nameof(GetLogsAsync)}) failed.");
            response.IsOk = false;
            response.Message = ex.Message;
        }

        return response;
    }

    private async Task<List<LaobianLog>> ReadLogsAsync(LaobianSite site, int days, int minLevel)
    {
        var result = new List<LaobianLog>();
        for (var i = 0; i <= days; i++)
        {
            var date = DateTime.Now.AddDays(-i);
            var logs = await _logFileService.GetLogsAsync(site, date);
            result.AddRange(logs.Where(x => (int) x.Level >= minLevel));
        }

        return result;
    }
}