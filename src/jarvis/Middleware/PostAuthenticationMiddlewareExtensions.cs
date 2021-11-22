using Microsoft.AspNetCore.Builder;

namespace Laobian.Jarvis.Middleware;

public static class PostAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UsePostAuthentication(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PostAuthenticationMiddleware>();
    }
}