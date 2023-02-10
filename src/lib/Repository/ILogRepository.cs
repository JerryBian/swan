using Swan.Core.Log;

namespace Swan.Lib.Repository
{
    public interface ILogRepository
    {
        List<SwanLog> ReadAll(LogLevel minLogLevel);

        void AddLog(SwanLog log);

        void Cleanup();
    }
}
