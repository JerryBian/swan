using System;

namespace Laobian.Share.Logger.File
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