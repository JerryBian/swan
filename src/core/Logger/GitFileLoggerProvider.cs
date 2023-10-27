using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Swan.Core.Logger
{
    internal class GitFileLoggerProvider : ILoggerProvider
    {
        private readonly ILogger _logger;
        private readonly IGitFileLoggerProcessor _processor;

        public GitFileLoggerProvider(IOptions<GitFileLoggerOption> option, IGitFileLoggerProcessor processor)
        {
            _processor = processor;
            _logger = new GitFileLogger(option.Value, processor);
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
