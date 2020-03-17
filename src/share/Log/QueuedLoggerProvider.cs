using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class QueuedLoggerProvider : ILoggerProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QueuedLoggerProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new QueuedLogger(_httpContextAccessor);
        }
    }
}