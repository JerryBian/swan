using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Swan.Core.Logger
{
    public static class GitFileLoggerExtension
    {
        public static ILoggingBuilder AddGitFile(this ILoggingBuilder builder, Action<GitFileLoggerOption> configure = null)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, GitFileLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<GitFileLoggerOption, GitFileLoggerProvider>(builder.Services);
            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            return builder;
        }
    }
}
