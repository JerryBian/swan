using Laobian.Share.Logger;
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
            ILaobianLogQueue laobianLogQueue, SystemLocker systemLocker)
        {
            _processor = new GitFileLoggerProcessor(options.Value, laobianLogQueue, systemLocker);
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