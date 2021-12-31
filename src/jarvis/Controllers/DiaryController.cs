using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site.Jarvis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.Controllers;

[Route("diary")]
public class DiaryController : Controller
{
    private readonly IDiaryGrpcService _diaryGrpcService;
    private readonly ILogger<DiaryController> _logger;
    private readonly JarvisOptions _options;

    public DiaryController(IOptions<JarvisOptions> option, ILogger<DiaryController> logger)
    {
        _logger = logger;
        _options = option.Value;
        _diaryGrpcService = GrpcClientHelper.CreateClient<IDiaryGrpcService>(option.Value.ApiLocalEndpoint);
    }


    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int page)
    {
        try
        {
            var viewModel = await GetViewModel(page, null, null);

            if (viewModel != null)
            {
                ViewData["Title"] = "所有日记";
                if (viewModel.CurrentPage > 1)
                {
                    ViewData["Title"] = ViewData["Title"].ToString() + $"：第{viewModel.CurrentPage}页";
                }

                return View(viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index page has error.");
        }

        return NotFound();
    }

    private async Task<PagedViewModel<DiaryRuntime>> GetViewModel(int page, int? year, int? month)
    {
        PagedViewModel<DiaryRuntime> viewModel = null;
        const int itemsPerPage = 2;
        var request = new DiaryGrpcRequest {Year = year, Month = month};
        var response = await _diaryGrpcService.GetDiaryDatesAsync(request);
        if (response.IsOk)
        {
            viewModel = new PagedViewModel<DiaryRuntime>(page, response.DiaryDates?.Count ?? 0, itemsPerPage)
                {Url = Request.Path};
            request.Count = itemsPerPage;
            request.ExtractRuntime = true;
            request.Offset = (viewModel.CurrentPage - 1) * itemsPerPage;
            response = await _diaryGrpcService.GetDiariesAsync(request);
            if (response.IsOk)
            {
                response.DiaryRuntimeList ??= new List<DiaryRuntime>();
                foreach (var diaryRuntime in response.DiaryRuntimeList)
                {
                    viewModel.Items.Add(diaryRuntime);
                }
            }
        }

        return viewModel;
    }

    [HttpGet]
    [Route("{year}")]
    public async Task<IActionResult> ListYear([FromRoute] int year, [FromQuery] int page)
    {
        try
        {
            var viewModel = await GetViewModel(page, year, null);

            if (viewModel != null)
            {
                return View("Index", viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ListYear page has error.");
        }

        return NotFound();
    }

    [HttpGet]
    [Route("{year}/{month}")]
    public async Task<IActionResult> ListMonth([FromRoute] int year, [FromRoute] int month, [FromQuery] int page)
    {
        try
        {
            var viewModel = await GetViewModel(page, year, month);

            if (viewModel != null)
            {
                return View("Index", viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ListYear page has error.");
        }

        return NotFound();
    }

    [HttpGet]
    [Route("{year}/{month}/{day}.html")]
    public async Task<IActionResult> Detail([FromRoute] int year, [FromRoute] int month, [FromRoute] int day)
    {
        var date = new DateTime(year, month, day);
        var request = new DiaryGrpcRequest {Date = date, ExtractRuntime = true, ExtractNext = true, ExtractPrev = true};
        var response = await _diaryGrpcService.GetDiaryAsync(request);
        if (response.IsOk)
        {
            if (response.NotFound)
            {
                return Redirect($"{_options.AdminRemoteEndpoint}/diary/add?date={date.ToDate()}");
            }

            return View(response.DiaryRuntime);
        }

        return NotFound();
    }
}