using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public static class InMemoryLoggerExtension
    {
        public static ILoggingBuilder AddInMemoryLogger(this ILoggingBuilder builder)
        {
            builder.AddProvider(new InMemoryLoggerProvider());
            return builder;
        }
    }
}
