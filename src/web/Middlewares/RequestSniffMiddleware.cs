using Microsoft.AspNetCore.Http.Extensions;
using Swan.Core.Extension;
using Swan.Core.Service;
using Swan.Core.Store;

namespace Swan.Web.Middlewares
{
    public class RequestSniffMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISwanStore _swanStore;
        private readonly ISwanService _swanService;
        private readonly ILogger<RequestSniffMiddleware> _logger;

        public RequestSniffMiddleware(
            RequestDelegate next,
            ISwanStore swanStore,
            ISwanService swanService,
            ILogger<RequestSniffMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _swanStore = swanStore;
            _swanService = swanService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.IsAuthorized())
            {
                _logger.LogInformation($"Admin Request => URL: {context.Request.GetDisplayUrl()}, IP: {context.GetIpAddress()}, Method: {context.Request.Method}");
            }

            await _next(context);

            if (context.Response.StatusCode == 200)
            {
                var path = context.Request.Path.Value;
                if (!string.IsNullOrEmpty(path) && path.Length > 1 && path.EndsWith('/'))
                {
                    path = path.TrimEnd('/');
                }
                await _swanStore.AddPageHitAsync(path, context.GetIpAddress());
            }
        }
    }
}
