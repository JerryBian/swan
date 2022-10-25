using Laobian.Lib.Log;

namespace Laobian.Lib.Service
{
    public interface ILogService
    {
        List<LaobianLog> ReadAll(LogLevel minLogLevel);

        void AddLog(LaobianLog log);

        void Cleanup();
    }
}
