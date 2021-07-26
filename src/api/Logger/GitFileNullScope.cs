using System;

namespace Laobian.Api.Logger
{
    public class GitFileNullScope : IDisposable
    {
        private GitFileNullScope()
        {
        }

        public static GitFileNullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}