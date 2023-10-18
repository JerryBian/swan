using Microsoft.Extensions.Logging;
using Swan.Core.Model;

namespace Swan.Core.Logger
{
    internal class GitFileLogger : ILogger
    {
        private readonly GitFileLoggerOption _option;
        private readonly IGitFileLoggerProcessor _processor;

        public GitFileLogger(GitFileLoggerOption option, IGitFileLoggerProcessor processor)
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
                Error = exception?.ToString(),
                Level = logLevel
            };
            _processor.Ingest(log);
        }
    }
}
