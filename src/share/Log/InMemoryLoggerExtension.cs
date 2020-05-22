using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public static class InMemoryLoggerExtension
    {
        public static ILoggingBuilder AddInMemoryLogger(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(new InMemoryLoggerProvider());
            return loggingBuilder;
        }
    }
}
