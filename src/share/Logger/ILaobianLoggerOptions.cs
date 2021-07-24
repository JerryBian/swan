using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger
{
    public interface ILaobianLoggerOptions
    {
        LogLevel MinLevel { get; set; }
    }
}