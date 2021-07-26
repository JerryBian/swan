using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Laobian.Share.Logger.Remote
{
    public static class RemoteLoggerExtension
    {
        public static ILoggingBuilder AddRemote(this ILoggingBuilder builder,
            Action<RemoteLoggerOptions> configure = null)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, RemoteLoggerProvider>());
            LoggerProviderOptions
                .RegisterProviderOptions<RemoteLoggerOptions, RemoteLoggerProvider>(builder.Services);
            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            return builder;
        }
    }
}