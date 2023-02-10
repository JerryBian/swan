using Swan.Core.Log;

namespace Swan.Lib.Service
{
    public interface ILogService
    {
        List<SwanLog> ReadAll(LogLevel minLogLevel);

        void AddLog(SwanLog log);

        void Cleanup();
    }
}
