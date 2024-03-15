using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Swan.Core.Logger
{
    public static class SwanLoggerExtension
    {
        public static ILoggingBuilder AddGitFile(this ILoggingBuilder builder, Action<SwanLoggerOption> configure = null)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, SwanLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<SwanLoggerOption, SwanLoggerProvider>(builder.Services);
            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            return builder;
        }
    }
}
