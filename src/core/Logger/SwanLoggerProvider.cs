using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Swan.Core.Logger
{
    internal class SwanLoggerProvider : ILoggerProvider
    {
        private readonly ILogger _logger;
        private readonly ISwanLoggerProcessor _processor;

        public SwanLoggerProvider(IOptions<SwanLoggerOption> option, ISwanLoggerProcessor processor)
        {
            _processor = processor;
            _logger = new SwanLogger(option.Value, processor);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }

        public void Dispose()
        {
            _processor.Dispose();
        }
    }
}
