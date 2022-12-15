using Swan.Lib.Model;
using Swan.Lib.Service;
using System.Net;

namespace Swan.Middlewares
{
    public class BlacklistIpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlacklistService _blacklistService;
        private readonly ILogger<BlacklistIpMiddleware> _logger;

        public BlacklistIpMiddleware(RequestDelegate next, ILogger<BlacklistIpMiddleware> logger, IBlacklistService blacklistService)
        {
            _next = next;
            _logger = logger;
            _blacklistService = blacklistService;
        }

        public async Task Invoke(HttpContext context)
        {
            List<BlacklistItem> items = await _blacklistService.GetAllAsync();
            IPAddress remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogDebug($"Request from remote ip: {remoteIp}");

            bool badIp = false;
            byte[] bytes = remoteIp.GetAddressBytes();
            foreach (BlacklistItem item in items)
            {
                if (item.InvalidTo > DateTime.Now && item.IpBytes.SequenceEqual(bytes))
                {
                    badIp = true;
                    break;
                }
            }

            if (badIp)
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
