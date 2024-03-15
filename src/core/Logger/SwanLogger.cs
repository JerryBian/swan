using Microsoft.Extensions.Logging;
using Swan.Core.Model;

namespace Swan.Core.Logger
{
    internal class SwanLogger : ILogger
    {
        private readonly SwanLoggerOption _option;
        private readonly ISwanLoggerProcessor _processor;

        public SwanLogger(SwanLoggerOption option, ISwanLoggerProcessor processor)
        {
            _option = option;
            _processor = processor;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _option.MinLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            SwanLog log = new()
            {
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                Level = logLevel.ToString()
            };
            _processor.Ingest(log);
        }
    }
}
