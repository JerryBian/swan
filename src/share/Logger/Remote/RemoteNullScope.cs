using System;

namespace Laobian.Share.Logger.Remote;

public class RemoteNullScope : IDisposable
{
    private RemoteNullScope()
    {
    }

    public static RemoteNullScope Instance { get; } = new();

    public void Dispose()
    {
    }
}