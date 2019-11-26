using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class QueuedLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var log = new LogEntry
            {
                Exception = exception,
                Message = formatter(state, exception),
                When = DateTime.Now
            };

            if (logLevel == LogLevel.Critical)
            {
                Global.CriticalLogQueue.Enqueue(log);
            }

            if (logLevel == LogLevel.Warning)
            {
                Global.WarningLogQueue.Enqueue(log);
            }

            if (logLevel == LogLevel.Error)
            {
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