using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Laobian.Lib.Log
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly FileLoggerOption _option;
        private readonly IFileLoggerProcessor _processor;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers;

        public FileLoggerProvider(IOptions<FileLoggerOption> option, IFileLoggerProcessor processor)
        {
            _option = option.Value;
            _processor = processor;
            _loggers = new ConcurrentDictionary<string, FileLogger>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, n =>
            {
                return new FileLogger(_option, _processor);
            });
        }

        public void Dispose()
        {
        }
    }
}
