using Laobian.Lib.Log;
using Laobian.Lib.Repository;

namespace Laobian.Lib.Service
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _repository;

        public LogService(ILogRepository logRepository)
        {
            _repository = logRepository;
        }

        public void AddLog(LaobianLog log)
        {
            _repository.AddLog(log);
        }

        public void Cleanup()
        {
            _repository.Cleanup();
        }

        public List<LaobianLog> ReadAll(LogLevel minLogLevel)
        {
            return _repository.ReadAll(minLogLevel);
        }
    }
}
