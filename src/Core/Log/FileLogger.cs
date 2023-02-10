namespace Swan.Core.Log
{
    public class FileLogger : ILogger
    {
        private readonly FileLoggerOption _option;
        private readonly IFileLoggerProcessor _processor;

        public FileLogger(FileLoggerOption option, IFileLoggerProcessor processor)
        {
            _option = option;
            _processor = processor;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _option.MinLogLevel <= logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            try
            {
                SwanLog log = new()
                {
                    Message = formatter(state, exception),
                    Exception = exception?.ToString(),
                    Level = logLevel,
                    Timestamp = DateTime.Now
                };
                _processor.Ingest(log);
            }
            catch { }
        }
    }
}
