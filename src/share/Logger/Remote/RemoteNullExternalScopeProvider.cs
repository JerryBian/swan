using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger.Remote;

public class RemoteNullExternalScopeProvider : IExternalScopeProvider
{
    private RemoteNullExternalScopeProvider()
    {
    }

    public static IExternalScopeProvider Instance { get; } = new RemoteNullExternalScopeProvider();

    public void ForEachScope<TState>(Action<object, TState> callback, TState state)
    {
    }

    public IDisposable Push(object state)
    {
        return RemoteNullScope.Instance;
    }
}