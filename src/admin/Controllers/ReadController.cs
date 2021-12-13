using System;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Admin.Controllers;

[Route("read")]
public class ReadController : Controller
{
    private readonly ApiSiteHttpClient _apiSiteHttpClient;
    private readonly ILogger<ReadController> _logger;

    public ReadController(ApiSiteHttpClient apiSiteHttpClient, ILogger<ReadController> logger)
    {
        _logger = logger;
        _apiSiteHttpClient = apiSiteHttpClient;
    }

    [HttpPost("stats")]
    public async Task<ApiResponse<ChartResponse>> GetReadStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var items = await _apiSiteHttpClient.GetReadItemsAsync();
            var chartResponse = new ChartResponse {Title = "xxxx", Type = "bar"};
            foreach (var item in items.GroupBy(x => x.StartTime.Year).OrderBy(x => x.Key))
            {
                chartResponse.Data.Add(item.Count());
                chartResponse.Labels.Add(item.Key.ToString());
            }

            response.Content = chartResponse;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get read stats failed.");
        }

        return response;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _apiSiteHttpClient.GetReadItemsAsync();
        return View(model);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ReadItem> Get([FromRoute] string id)
    {
        return await _apiSiteHttpClient.GetReadItemAsync(id);
    }

    [HttpGet("add")]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<ApiResponse<object>> Add([FromForm] ReadItem readItem)
    {
        var response = new ApiResponse<object>();
        try
        {
            readItem.IsCompleted = Request.Form["isCompleted"] == "on";
            readItem.IsPublished = Request.Form["isPublished"] == "on";
            await _apiSiteHttpClient.AddReadItemAsync(readItem);
            response.RedirectTo = "/read";
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add new read failed. {JsonUtil.Serialize(readItem)}");
        }

        return response;
    }

    [HttpGet("{id}/update")]
    public async Task<IActionResult> Update([FromRoute] string id)
    {
        var bookItem = await _apiSiteHttpClient.GetReadItemAsync(id);
        if (bookItem == null)
        {
            return NotFound();
        }

        return View(bookItem);
    }

    [HttpPut]
    public async Task<ApiResponse<object>> Update([FromForm] ReadItem readItem)
    {
        var response = new ApiResponse<object>();
        try
        {
            readItem.IsCompleted = Request.Form["isCompleted"] == "on";
            readItem.IsPublished = Request.Form["isPublished"] == "on";
            await _apiSiteHttpClient.UpdateReadItemAsync(readItem);
            response.RedirectTo = "/read";
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update read failed. {JsonUtil.Serialize(readItem)}");
        }

        return response;
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ApiResponse<object>> Delete([FromRoute] string id)
    {
        var response = new ApiResponse<object>();
        try
        {
            await _apiSiteHttpClient.DeleteReadItemAsync(id);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Delete read {id} failed.");
        }

        return response;
    }
}