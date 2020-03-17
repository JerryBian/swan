using System;
using Laobian.Share.Blog.Alert;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class QueuedLogger : ILogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QueuedLogger(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            var log = new BlogAlertEntry
            {
                Message = formatter(state, exception),
                When = DateTime.Now,
                Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                RequestUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl(),
                UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"]
            };

            if (logLevel == LogLevel.Warning)
            {
                Global.WarningLogQueue.Enqueue(log);
            }

            if (logLevel == LogLevel.Error)
            {
                log.Exception = exception;
                Global.ErrorLogQueue.Enqueue(log);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Warning;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}