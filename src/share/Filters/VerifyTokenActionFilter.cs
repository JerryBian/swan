using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Laobian.Share.Filters;

public class VerifyTokenActionFilter : IAsyncActionFilter
{
    private readonly string _token;
    private readonly string[] _urlPrefixes;

    public VerifyTokenActionFilter(string token, string[] urlPrefixes = null)
    {
        _token = token;
        _urlPrefixes = urlPrefixes;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<VerifyTokenActionFilter>>();
        if (_urlPrefixes == null || _urlPrefixes.Any(x =>
                context.HttpContext.Request.Path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase)))
        {
            if (!context.HttpContext.Request.Headers.ContainsKey(Constants.ApiRequestHeaderToken))
            {
                logger?.LogError(
                    $"No API token set. IP: {context.HttpContext.Connection.RemoteIpAddress}, User Agent: {context.HttpContext.Request.Headers[HeaderNames.UserAgent]}");
                context.Result = new BadRequestObjectResult("No API token set.");
                return;
            }

            if (_token != context.HttpContext.Request.Headers[Constants.ApiRequestHeaderToken])
            {
                logger?.LogError(
                    $"Invalid API token set: {context.HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}, User Agent: {context.HttpContext.Request.Headers[HeaderNames.UserAgent]}");
                context.Result = new BadRequestObjectResult(
                    $"Invalid API token set: {context.HttpContext.Request.Headers[Constants.ApiRequestHeaderToken]}.");
                return;
            }
        }

        await next();
    }
}