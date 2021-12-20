using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("diary")]
public class DiaryController : Controller
{
    private readonly IDiaryGrpcService _diaryGrpcService;
    private readonly ILogger<DiaryController> _logger;
    private readonly AdminOptions _options;

    public DiaryController(IOptions<AdminOptions> option, ILogger<DiaryController> logger)
    {
        _logger = logger;
        _options = option.Value;
        _diaryGrpcService = GrpcClientHelper.CreateClient<IDiaryGrpcService>(option.Value.ApiLocalEndpoint);
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("chart/count-per-year")]
    public async Task<ApiResponse<ChartResponse>> GetChartForCountPerYear()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var request = new DiaryGrpcRequest();
            var diaryResponse = await _diaryGrpcService.GetDiaryDatesAsync(request);
            if (diaryResponse.IsOk)
            {
                var chart = new ChartResponse
                {
                    Title = "当年的日记数",
                    Type = "line"
                };

                diaryResponse.DiaryDates ??= new List<DateTime>();
                foreach (var item in diaryResponse.DiaryDates.GroupBy(x => x.Year).OrderBy(x => x.Key))
                {
                    chart.Data.Add(item.Count());
                    chart.Labels.Add($"{item.Key}年");
                }

                response.Content = chart;
            }
            else
            {
                response.IsOk = false;
                response.Message = diaryResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get chart for count per year failed.");
        }

        return response;
    }

    [HttpPost("chart/count-per-month")]
    public async Task<ApiResponse<ChartResponse>> GetChartForCountPerMonth()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var request = new DiaryGrpcRequest{Year = DateTime.Now.Year};
            var diaryResponse = await _diaryGrpcService.GetDiaryDatesAsync(request);
            if (diaryResponse.IsOk)
            {
                var chart = new ChartResponse
                {
                    Title = "当月的日记数",
                    Type = "line"
                };

                diaryResponse.DiaryDates ??= new List<DateTime>();
                foreach (var item in diaryResponse.DiaryDates.GroupBy(x => x.Month).OrderBy(x => x.Key))
                {
                    chart.Data.Add(item.Count());
                    chart.Labels.Add($"{item.Key}月");
                }

                response.Content = chart;
            }
            else
            {
                response.IsOk = false;
                response.Message = diaryResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get chart for count per month failed.");
        }

        return response;
    }

    [HttpPost("chart/words-per-month")]
    public async Task<ApiResponse<ChartResponse>> GetChartForWordsPerMonth()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var request = new DiaryGrpcRequest { Year = DateTime.Now.Year, ExtractRuntime = true};
            var diaryResponse = await _diaryGrpcService.GetDiariesAsync(request);
            if (diaryResponse.IsOk)
            {
                var chart = new ChartResponse
                {
                    Title = "当月的日记字数",
                    Type = "line"
                };

                diaryResponse.DiaryRuntimeList ??= new List<DiaryRuntime>();
                foreach (var item in diaryResponse.DiaryRuntimeList.GroupBy(x => x.Raw.Date.Month).OrderBy(x => x.Key))
                {
                    chart.Data.Add(item.Sum(x => x.WordsCount));
                    chart.Labels.Add($"{item.Key}月");
                }

                response.Content = chart;
            }
            else
            {
                response.IsOk = false;
                response.Message = diaryResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get chart for words per month failed.");
        }

        return response;
    }

    [HttpPost("chart/words-per-year")]
    public async Task<ApiResponse<ChartResponse>> GetChartForWordsPerYear()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var request = new DiaryGrpcRequest { ExtractRuntime = true };
            var diaryResponse = await _diaryGrpcService.GetDiariesAsync(request);
            if (diaryResponse.IsOk)
            {
                var chart = new ChartResponse
                {
                    Title = "当年的日记字数",
                    Type = "line"
                };

                diaryResponse.DiaryRuntimeList ??= new List<DiaryRuntime>();
                foreach (var item in diaryResponse.DiaryRuntimeList.GroupBy(x => x.Raw.Date.Year).OrderBy(x => x.Key))
                {
                    chart.Data.Add(item.Sum(x => x.WordsCount));
                    chart.Labels.Add($"{item.Key}年");
                }

                response.Content = chart;
            }
            else
            {
                response.IsOk = false;
                response.Message = diaryResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get chart for words per year failed.");
        }

        return response;
    }

    [HttpGet]
    [Route("add")]
    public IActionResult AddDiary([FromQuery] DateTime date)
    {
        if (date == default)
        {
            date = DateTime.Now;
        }

        ViewData["Date"] = date.ToDate();
        return View();
    }

    [HttpPost]
    [Route("add")]
    public async Task<ApiResponse<object>> AddDiary([FromForm] Diary diary)
    {
        if (diary.Date == default)
        {
            diary.Date = DateTime.Now;
        }

        var response = new ApiResponse<object>();
        try
        {
            var request = new DiaryGrpcRequest { Diary = diary };
            var diaryResponse = await _diaryGrpcService.AddDiaryAsync(request);
            if (diaryResponse.IsOk)
            {
                response.RedirectTo = diaryResponse.Diary.GetFullPath(_options);
            }
            else
            {
                response.IsOk = false;
                response.Message = diaryResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add diary failed: {JsonUtil.Serialize(diary)}");
        }

        return response;
    }

    [HttpGet]
    [Route("update")]
    public async Task<IActionResult> UpdateDiary([FromQuery]DateTime date)
    {
        var request = new DiaryGrpcRequest {Date = date};
        try
        {
            var response = await _diaryGrpcService.GetDiaryAsync(request);
            if (response.IsOk)
            {
                if (!response.NotFound)
                {
                    return View(response.DiaryRuntime.Raw);
                }
                else
                {
                    return Redirect($"/diary/add?date={date.ToDate()}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Update page has error: {date}");
        }

        return NotFound();
    }

    [HttpPut]
    [Route("update")]
    public async Task<ApiResponse<object>> UpdateDiary([FromForm] Diary diary)
    {
        var response = new ApiResponse<object>();
        try
        {
            var request = new DiaryGrpcRequest { Diary = diary };
            var diaryResponse = await _diaryGrpcService.UpdateDiaryAsync(request);
            if (diaryResponse.IsOk)
            {
                response.RedirectTo = diaryResponse.Diary.GetFullPath(_options);
            }
            else
            {
                response.IsOk = false;
                response.Message = diaryResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update diary failed: {JsonUtil.Serialize(diary)}");
        }

        return response;
    }
}