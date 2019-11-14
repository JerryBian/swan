using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class QueuedLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var log = new LogEntry
            {
                Exception = exception,
                Level = logLevel,
                Message = formatter(state, exception),
                When = DateTime.Now
            };

            if (logLevel == LogLevel.Critical)
            {
                MemoryStore.CriticalLogQueue.Enqueue(log);
            }

            if (logLevel == LogLevel.Warning)
            {
                MemoryStore.WarningLogQueue.Enqueue(log);
            }

            if (logLevel == LogLevel.Error)
            {
                MemoryStore.ErrorLogQueue.Enqueue(log);
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
