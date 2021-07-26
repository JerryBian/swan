using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLoggerOptions : ILaobianLoggerOptions
    {
        public string LoggerName { get; set; }
        public LogLevel MinLevel { get; set; } = LogLevel.Information;
    }
}