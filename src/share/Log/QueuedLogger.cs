using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class QueuedLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            MemoryStore.LogQueue.Enqueue(new LogEntry
            {
                Exception = exception,
                Level =  logLevel,
                Message = formatter(state, exception),
                When = DateTime.Now
            });
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
