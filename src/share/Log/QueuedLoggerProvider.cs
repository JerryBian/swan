using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class QueuedLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new QueuedLogger();
        }
    }
}