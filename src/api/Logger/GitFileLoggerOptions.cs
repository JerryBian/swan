using Laobian.Share.Logger;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Logger
{
    public class GitFileLoggerOptions : ILaobianLoggerOptions
    {
        public string BaseDir { get; set; }

        public string LoggerName { get; set; }

        public LogLevel MinLevel { get; set; } = LogLevel.Information;
    }
}