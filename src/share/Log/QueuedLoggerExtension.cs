using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public static class QueuedLoggerExtension
    {
        public static ILoggerFactory AddQueuedLogger(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new QueuedLoggerProvider());
            return loggerFactory;
        }
    }
}
