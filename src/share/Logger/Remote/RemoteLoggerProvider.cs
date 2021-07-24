using Laobian.Share.Logger.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly GitFileLogger _logger;
        private readonly GitFileLoggerProcessor _processor;
        private IExternalScopeProvider _externalScopeProvider;

        public RemoteLoggerProvider(IOptions<GitFileLoggerOptions> options)
        {
            _processor = new GitFileLoggerProcessor(options.Value);
            _logger = new GitFileLogger(_processor)
            {
                Options = options.Value,
                ScopeProvider = _externalScopeProvider
            };
            _externalScopeProvider = RemoteNullExternalScopeProvider.Instance;
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