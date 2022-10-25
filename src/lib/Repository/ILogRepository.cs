using Laobian.Lib.Log;

namespace Laobian.Lib.Repository
{
    public interface ILogRepository
    {
        List<LaobianLog> ReadAll(LogLevel minLogLevel);

        void AddLog(LaobianLog log);

        void Cleanup();
    }
}
