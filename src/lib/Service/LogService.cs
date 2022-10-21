using Laobian.Lib.Helper;
using Laobian.Lib.Log;
using Laobian.Lib.Provider;
using Laobian.Lib.Repository;
using System.Text;

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

        public List<LaobianLog> ReadAllAsync(LogLevel minLogLevel)
        {
            return _repository.ReadAllAsync(minLogLevel);
        }
    }
}
