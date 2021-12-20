using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("log")]
public class LogController : Controller
{
    private readonly ILogger<LogController> _logger;
    private readonly ILogGrpcService _logGrpcService;

    public LogController(IOptions<AdminOptions> options, ILogger<LogController> logger)
    {
        _logger = logger;
        _logGrpcService = GrpcClientHelper.CreateClient<ILogGrpcService>(options.Value.ApiLocalEndpoint);
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("/log")]
    public async Task<ApiResponse<List<LaobianLog>>> GetLogs([FromQuery] string site, [FromQuery] int minLevel,
        [FromQuery] int days)
    {
        var response = new ApiResponse<List<LaobianLog>>();
        try
        {
            var request = new LogGrpcRequest
            {
                Days = days,
                Logger = site,
                MinLevel = minLevel
            };
            var logResponse = await _logGrpcService.GetLogsAsync(request);
            if (logResponse.IsOk)
            {
                logResponse.Logs = logResponse.Logs ?? new List<LaobianLog>();
                var logs = logResponse.Logs.OrderByDescending(x => x.TimeStamp).ToList();
                response.Content = logs;
            }
            else
            {
                response.IsOk = false;
                response.Message = logResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get logs failed. {site}:{minLevel}:{days}");
        }

        return response;
    }

    [HttpPost]
    [Route("/log/stats")]
    public async Task<ApiResponse<ChartResponse>> GetLogStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var request = new LogGrpcRequest
            {
                Days = 14,
                Logger = "all",
                MinLevel = 3
            };
            var logResponse = await _logGrpcService.GetLogsAsync(request);
            if (logResponse.IsOk)
            {
                logResponse.Logs = logResponse.Logs ?? new List<LaobianLog>();
                var groupedLogs = logResponse.Logs.GroupBy(x => x.TimeStamp.Date).OrderBy(x => x.Key);
                var chartResponse = new ChartResponse {Title = "# Logs are warning and error", Type = "line"};
                foreach (var item in groupedLogs)
                {
                    chartResponse.Labels.Add(item.Key.ToRelativeDaysHuman());
                    chartResponse.Data.Add(item.Count());
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = logResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get logs stats failed.");
        }

        return response;
    }
}