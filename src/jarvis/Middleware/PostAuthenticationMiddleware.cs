using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.Middleware;

public class PostAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public PostAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, IOptions<JarvisOptions> option)
    {
        if (!httpContext.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCultureIgnoreCase) &&
            httpContext.User.Identity?.IsAuthenticated != true)
        {
            httpContext.Response.Redirect(
                $"{option.Value.AdminRemoteEndpoint}/login?returnUrl={httpContext.Request.GetDisplayUrl()}");
            return;
        }

        await _next(httpContext);
    }
}