using Laobian.Share.Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Logger
{
    public class GitFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly GitFileLogger _logger;
        private IExternalScopeProvider _externalScopeProvider;

        public GitFileLoggerProvider(IOptions<GitFileLoggerOptions> options,
            ILaobianLogQueue laobianLogQueue)
        {
            _logger = new GitFileLogger(laobianLogQueue)
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
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _externalScopeProvider = scopeProvider;
            _logger.ScopeProvider = scopeProvider;
        }
    }
}