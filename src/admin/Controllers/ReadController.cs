using System;
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
    private readonly ILogger<ReadController> _logger;
    private readonly ApiSiteHttpClient _apiSiteHttpClient;

    public ReadController(ApiSiteHttpClient apiSiteHttpClient, ILogger<ReadController> logger)
    {
        _logger = logger;
        _apiSiteHttpClient = apiSiteHttpClient;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _apiSiteHttpClient.GetReadItemsAsync();
        return View(model);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<BookItem> Get([FromRoute] string id)
    {
        return await _apiSiteHttpClient.GetReadItemAsync(id);
    }

    [HttpGet("add")]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<ApiResponse<object>> Add([FromForm] BookItem bookItem)
    {
        var response = new ApiResponse<object>();
        try
        {
            bookItem.IsCompleted = Request.Form["isCompleted"] == "on";
            bookItem.IsPublished = Request.Form["isPublished"] == "on";
            await _apiSiteHttpClient.AddBookItemAsync(bookItem);
            response.RedirectTo = "/read";
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add new read failed. {JsonUtil.Serialize(bookItem)}");
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

    [HttpPost("update")]
    public async Task<ApiResponse<object>> Update([FromForm] BookItem bookItem)
    {
        var response = new ApiResponse<object>();
        try
        {
            bookItem.IsCompleted = Request.Form["isCompleted"] == "on";
            bookItem.IsPublished = Request.Form["isPublished"] == "on";
            await _apiSiteHttpClient.UpdateBookItemAsync(bookItem);
            response.RedirectTo = "/read";
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update read failed. {JsonUtil.Serialize(bookItem)}");
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
            await _apiSiteHttpClient.DeleteBookItemAsync(id);
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