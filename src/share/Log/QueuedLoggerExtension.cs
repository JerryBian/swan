using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public static class QueuedLoggerExtension
    {
        public static ILoggingBuilder AddQueuedLogger(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(new QueuedLoggerProvider());
            return loggingBuilder;
        }
    }
}
