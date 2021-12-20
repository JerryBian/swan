using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Read;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("read")]
public class ReadController : Controller
{
    private readonly IReadGrpcService _readGrpcService;
    private readonly ILogger<ReadController> _logger;

    public ReadController(IOptions<AdminOptions> options, ILogger<ReadController> logger)
    {
        _logger = logger;
        _readGrpcService = GrpcClientHelper.CreateClient<IReadGrpcService>(options.Value.ApiLocalEndpoint);
    }

    [HttpPost("stats")]
    public async Task<ApiResponse<ChartResponse>> GetReadStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var request = new ReadGrpcRequest();
            var itemsResponse = await _readGrpcService.GetReadItemsAsync(request);
            if (itemsResponse.IsOk)
            {
                var chartResponse = new ChartResponse { Title = "当年阅读数", Type = "bar" };
                foreach (var item in itemsResponse.ReadItems.GroupBy(x => x.Raw.StartTime.Year).OrderBy(x => x.Key))
                {
                    chartResponse.Data.Add(item.Count());
                    chartResponse.Labels.Add(item.Key.ToString());
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = itemsResponse.Message;
            }
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
        try
        {
            var request = new ReadGrpcRequest();
            var itemsResponse = await _readGrpcService.GetReadItemsAsync(request);
            if (itemsResponse.IsOk)
            {
                return View(itemsResponse.ReadItems ?? new List<ReadItemRuntime>());
            }
            else
            {
                return NotFound(itemsResponse.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index page error");
            return NotFound(ex.Message);
        }
        
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
            var readRequest = new ReadGrpcRequest { ReadItem = readItem };
            var readResponse = await _readGrpcService.AddReadItemAsync(readRequest);
            if (readResponse.IsOk)
            {
                response.RedirectTo = "/read";
            }
            else
            {
                response.IsOk = false;
                response.Message = readResponse.Message;
            }
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
        var readRequest = new ReadGrpcRequest { ReadItemId = id};
        var readResponse = await _readGrpcService.GetReadItemAsync(readRequest);
        if (readResponse.IsOk)
        {
            return View(readResponse.ReadItemRuntime.Raw);
        }

        return NotFound(readResponse.Message);
    }

    [HttpPut]
    public async Task<ApiResponse<object>> Update([FromForm] ReadItem readItem)
    {
        var response = new ApiResponse<object>();
        try
        {
            readItem.IsCompleted = Request.Form["isCompleted"] == "on";
            readItem.IsPublished = Request.Form["isPublished"] == "on";
            var readRequest = new ReadGrpcRequest { ReadItem = readItem };
            var readResponse = await _readGrpcService.UpdateReadItemAsync(readRequest);
            if (readResponse.IsOk)
            {
                response.RedirectTo = "/read";
            }
            else
            {
                response.IsOk = false;
                response.Message = readResponse.Message;
            }
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
            var readRequest = new ReadGrpcRequest { ReadItemId = id};
            var readResponse = await _readGrpcService.DeleteReadItemAsync(readRequest);
            if (!readResponse.IsOk)
            {
                response.IsOk = false;
                response.Message = readResponse.Message;
            }
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