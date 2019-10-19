using System;
using System.Collections.Generic;
using System.Text;
using Laobian.Share.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Extension
{
    public static class EmailLoggerFactoryExtension
    {
        public static ILoggingBuilder AddEmail(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, EmailLoggerProvider>();
            return builder;
        }
    }
}
