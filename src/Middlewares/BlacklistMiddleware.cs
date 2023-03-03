using Swan.Core.Extension;
using Swan.Core.Store;
using System.Net;

namespace Swan.Middlewares
{
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlacklistStore _blacklistStore;
        private readonly ILogger<BlacklistMiddleware> _logger;

        public BlacklistMiddleware(RequestDelegate next, ILogger<BlacklistMiddleware> logger, IBlacklistStore blacklistService)
        {
            _next = next;
            _logger = logger;
            _blacklistStore = blacklistService;
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = context.GetIpAddress();
            _logger.LogDebug($"Request from remote ip: {remoteIp}");

            if (_blacklistStore.Has(remoteIp))
            {
                _logger.LogWarning(
                    "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _next.Invoke(context);
        }
    }
}
