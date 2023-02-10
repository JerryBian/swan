using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

namespace Swan.Core.Log
{
    public static class FileLoggerExtension
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder,
            Action<FileLoggerOption> configure = null)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
            LoggerProviderOptions
                .RegisterProviderOptions<FileLoggerOption, FileLoggerProvider>(builder.Services);
            if (configure != null)
            {
                _ = builder.Services.Configure(configure);
            }

            return builder;
        }
    }
}
