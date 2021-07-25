using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger.File
{
    public class GitFileLoggerOptions : ILaobianLoggerOptions
    {
        public string LoggerName { get; set; }

        public string LoggerDir { get; set; }

        public LogLevel MinLevel { get; set; } = LogLevel.Information;
    }
}