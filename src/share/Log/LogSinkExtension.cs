using System;
using Serilog;
using Serilog.Configuration;

namespace Laobian.Share.Log
{
    public static class LogSinkExtension
    {
        public static LoggerConfiguration QueuedLogSink(this LoggerSinkConfiguration config,
            IFormatProvider formatProvider = null)
        {
            return config.Sink(new QueuedLogSink(formatProvider));
        }
    }
}
