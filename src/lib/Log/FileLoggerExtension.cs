using Laobian.Lib.Option;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

namespace Laobian.Lib.Log
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
                builder.Services.Configure(configure);
            }

            return builder;
        }
    }
}
