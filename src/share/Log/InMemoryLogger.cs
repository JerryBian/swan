using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class InMemoryLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = $"{formatter(state, exception)}";
            if (logLevel == LogLevel.Warning)
            {
                Global.InMemoryLogQueue.Enqueue(new LogEntry
                {
                    Exception =  exception?.ToString(),
                    Level = LogLevel.Warning,
                    Message = message,
                    When = DateTime.Now
                });
            }
            else if(logLevel == LogLevel.Information || logLevel == LogLevel.Debug)
            {
                Global.InMemoryLogQueue.Enqueue(new LogEntry
                {
                    Exception = exception?.ToString(),
                    Level = LogLevel.Information,
                    Message = message,
                    When = DateTime.Now
                });
            }
            else
            {
                Global.InMemoryLogQueue.Enqueue(new LogEntry
                {
                    Exception = exception?.ToString(),
                    Level = LogLevel.Error,
                    Message = message,
                    When = DateTime.Now
                });
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
