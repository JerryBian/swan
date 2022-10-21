using Laobian.Lib.Log;

namespace Laobian.Lib.Repository
{
    public interface ILogRepository
    {
        List<LaobianLog> ReadAllAsync(LogLevel minLogLevel);

        void AddLog(LaobianLog log);
    }
}
