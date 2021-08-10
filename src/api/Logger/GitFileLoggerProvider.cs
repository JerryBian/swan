using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Logger
{
    public class GitFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly GitFileLogger _logger;
        private readonly GitFileLoggerProcessor _processor;
        private IExternalScopeProvider _externalScopeProvider;

        public GitFileLoggerProvider(IOptions<GitFileLoggerOptions> options,
            IGitFileLogQueue gitFileLogQueue)
        {
            _processor = new GitFileLoggerProcessor(options.Value, gitFileLogQueue);
            _logger = new GitFileLogger(_processor)
            {
                Options = options.Value,
                ScopeProvider = _externalScopeProvider
            };
            _externalScopeProvider = GitFileNullExternalScopeProvider.Instance;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }

        public void Dispose()
        {
            _processor?.Dispose();
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _externalScopeProvider = scopeProvider;
            _logger.ScopeProvider = scopeProvider;
        }
    }
}