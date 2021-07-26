using Laobian.Share.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly RemoteLogger _logger;
        private readonly RemoteLoggerProcessor _processor;
        private IExternalScopeProvider _externalScopeProvider;

        public RemoteLoggerProvider(IOptions<RemoteLoggerOptions> options, IRemoteLoggerSink sink)
        {
            _processor = new RemoteLoggerProcessor(options.Value, sink);
            _logger = new RemoteLogger(_processor)
            {
                Options = options.Value,
                ScopeProvider = _externalScopeProvider
            };
            _externalScopeProvider = RemoteNullExternalScopeProvider.Instance;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (StringHelper.EqualIgnoreCase("System.Net.Http.HttpClient.log.ClientHandler", categoryName) ||
                StringHelper.EqualIgnoreCase("System.Net.Http.HttpClient.log.LogicalHandler", categoryName))
            {
                return NullLogger.Instance;
            }

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