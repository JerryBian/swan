using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger.Remote
{
    public class RemoteLoggerOptions : ILaobianLoggerOptions
    {
        public LogLevel MinLevel { get; set; } = LogLevel.Information;
    }
}