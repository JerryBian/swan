using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Site;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMiscGrpcService _miscGrpcService;

    public HomeController(ILogger<HomeController> logger, IOptions<AdminOptions> options)
    {
        _logger = logger;
        _miscGrpcService = GrpcClientHelper.CreateClient<IMiscGrpcService>(options.Value.ApiLocalEndpoint);
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    [HttpPost]
    [Route("persistent")]
    public async Task PersistentAsync()
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var message = await reader.ReadToEndAsync();
            await _miscGrpcService.PersistentGitFileAsync(new MiscGrpcRequest { Message = message });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Persistent DB failed.");
        }
    }

    [HttpPost("git-file-stat")]
    public async Task<ApiResponse<List<GitFileStat>>> GetGitFileStatsAsync()
    {
        var apiResponse = new ApiResponse<List<GitFileStat>>();
        try
        {
            var response = await _miscGrpcService.GetDbStatAsync(new MiscGrpcRequest());
            if (response.IsOk)
            {
                apiResponse.Content = response.DbStats ?? new List<GitFileStat>();
            }
            else
            {
                apiResponse.IsOk = false;
                apiResponse.Message = response.Message;
            }
        }
        catch (Exception ex)
        {
            apiResponse.IsOk = false;
            apiResponse.Message = ex.Message;
            _logger.LogError(ex, "Get Git File stats failed.");
        }

        return apiResponse;
    }

    [HttpPost("site-stat")]
    public async Task<ApiResponse<SiteStat>> GetSiteStatAsync([FromQuery] string site)
    {
        var apiResponse = new ApiResponse<SiteStat>();
        try
        {
            if (Enum.TryParse<LaobianSite>(site, true, out var s))
            {
                if (s == LaobianSite.Admin)
                {
                    apiResponse.Content = SiteStatHelper.Get();
                }
                else
                {
                    var response = await _miscGrpcService.GetSiteStatAsync(new MiscGrpcRequest { Site = s });
                    if (response.IsOk)
                    {
                        apiResponse.Content = response.SiteStat;
                    }
                    else
                    {
                        apiResponse.IsOk = false;
                        apiResponse.Message = response.Message;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            apiResponse.IsOk = false;
            apiResponse.Message = ex.Message;
            _logger.LogError(ex, $"Get site {site} stat failed.");
        }

        return apiResponse;
    }
}