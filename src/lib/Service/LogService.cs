using Swan.Core.Log;
using Swan.Lib.Repository;

namespace Swan.Lib.Service
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _repository;

        public LogService(ILogRepository logRepository)
        {
            _repository = logRepository;
        }

        public void AddLog(SwanLog log)
        {
            _repository.AddLog(log);
        }

        public void Cleanup()
        {
            _repository.Cleanup();
        }

        public List<SwanLog> ReadAll(LogLevel minLogLevel)
        {
            return _repository.ReadAll(minLogLevel);
        }
    }
}
