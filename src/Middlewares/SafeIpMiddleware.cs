namespace Laobian.Middlewares
{
    public class SafeIpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SafeIpMiddleware> _logger;

        public SafeIpMiddleware(RequestDelegate next, ILogger<SafeIpMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
    }
}
