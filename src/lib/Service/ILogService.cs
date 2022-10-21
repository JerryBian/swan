using Laobian.Lib.Log;

namespace Laobian.Lib.Service
{
    public interface ILogService
    {
        List<LaobianLog> ReadAllAsync(LogLevel minLogLevel);

        void AddLog(LaobianLog log);
    }
}
