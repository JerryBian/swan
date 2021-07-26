using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Logger
{
    public class GitFileNullExternalScopeProvider : IExternalScopeProvider
    {
        private GitFileNullExternalScopeProvider()
        {
        }

        public static IExternalScopeProvider Instance { get; } = new GitFileNullExternalScopeProvider();

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }

        public IDisposable Push(object state)
        {
            return GitFileNullScope.Instance;
        }
    }
}