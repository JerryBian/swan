using Laobian.Lib.Service;
using System.Net;

namespace Laobian.Middlewares
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
            var items = await _blacklistService.GetAllAsync();
            var remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogDebug($"Request from remote ip: {remoteIp}");

            var badIp = false;
            var bytes = remoteIp.GetAddressBytes();
            foreach(var item in items)
            {
                if(item.InvalidTo < DateTime.Now && item.IpBytes.SequenceEqual(bytes))
                {
                    badIp = true;
                    break;
                }
            }

            if(badIp)
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
