using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Option;
using Swan.Core.Service;
using Swan.Core.Store;
using System.Net;

namespace Swan.Web.Middlewares
{
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISwanStore _swanStore;
        private readonly ISwanService _swanService;
        private readonly SwanOption _options;
        private readonly ILogger<BlacklistMiddleware> _logger;

        public BlacklistMiddleware(
            RequestDelegate next,
            ISwanStore swanStore,
            ISwanService swanService,
            IOptions<SwanOption> options,
            ILogger<BlacklistMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
            _swanStore = swanStore;
            _swanService = swanService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.GetIpAddress();
            if (await _swanStore.IsInBlacklistAsync(ip))
            {
                var activityId = StringHelper.Random();
                _logger.LogWarning($"IP {ip} is disallowed to visit any page {context.Request.GetDisplayUrl()} due to it's in blacklist. Activity {activityId}.");
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync($"Forbidden. Contact {_options.ContactEmail} with id {activityId}.");
                return;
            }

            await _next(context);
        }
    }
}
