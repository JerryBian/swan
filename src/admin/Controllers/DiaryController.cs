using System;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Site.Jarvis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("diary")]
public class DiaryController : Controller
{
    private readonly ApiSiteHttpClient _httpClient;
    private readonly ILogger<DiaryController> _logger;
    private readonly AdminOptions _options;

    public DiaryController(IOptions<AdminOptions> option, ApiSiteHttpClient httpClient, ILogger<DiaryController> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
        _options = option.Value;
    }

    [HttpPost("chart/count-per-year")]
    public async Task<ApiResponse<ChartResponse>> GetChartForCountPerYear()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var diaries = await _httpClient.GetDiariesAsync();
            var chart = new ChartResponse
            {
                Title = "每年的日志数",
                Type = "line"
            };
            foreach (var item in diaries.GroupBy(x => x.Raw.CreateTime.Year).OrderBy(x => x.Key))
            {
                chart.Data.Add(item.Count());
                chart.Labels.Add(item.Key.ToString());
            }

            response.Content = chart;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get chart for count per year failed.");
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
    public async Task<IActionResult> AddDiary([FromForm] Diary diary)
    {
        if (diary.Date == default)
        {
            diary.Date = DateTime.Now;
        }

        await _httpClient.AddDiaryAsync(diary);
        return Redirect(diary.GetFullPath(_options));
    }

    [HttpGet]
    [Route("update/{date}")]
    public async Task<IActionResult> UpdateDiary(DateTime date)
    {
        var item = await _httpClient.GetDiaryAsync(date);
        if (item == null)
        {
            return Redirect($"/diary/add?date={date.ToDate()}");
        }

        return View(item);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> UpdateDiary([FromForm] Diary diary)
    {
        await _httpClient.UpdateDiaryAsync(diary);
        return Redirect(diary.GetFullPath(_options));
    }
}