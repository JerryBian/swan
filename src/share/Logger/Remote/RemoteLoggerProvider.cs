using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly RemoteLogger _logger;
        private IExternalScopeProvider _externalScopeProvider;

        public RemoteLoggerProvider(IOptions<RemoteLoggerOptions> options, ILaobianLogQueue logQueue)
        {
            _logger = new RemoteLogger(logQueue)
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
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _externalScopeProvider = scopeProvider;
            _logger.ScopeProvider = scopeProvider;
        }
    }
}