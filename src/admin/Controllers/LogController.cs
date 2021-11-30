using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Admin.Controllers;

[Route("log")]
public class LogController : Controller
{
    private readonly ApiSiteHttpClient _apiSiteHttpClient;
    private readonly ILogger<LogController> _logger;

    public LogController(ApiSiteHttpClient apiSiteHttpClient, ILogger<LogController> logger)
    {
        _logger = logger;
        _apiSiteHttpClient = apiSiteHttpClient;
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
            var logs = await _apiSiteHttpClient.GetLogsAsync(site, days, minLevel);
            logs = logs.OrderByDescending(x => x.TimeStamp).ToList();
            response.Content = logs;
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
            var logs = await _apiSiteHttpClient.GetLogsAsync("all", 14, 3);
            var groupedLogs = logs.GroupBy(x => x.TimeStamp.Date).OrderBy(x => x.Key);
            var chartResponse = new ChartResponse {Title = "# Logs are warning and error", Type = "line"};
            foreach (var item in groupedLogs)
            {
                chartResponse.Labels.Add(item.Key.ToRelativeDaysHuman());
                chartResponse.Data.Add(item.Count());
            }

            response.Content = chartResponse;
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