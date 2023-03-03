using Swan.Core.Model.Object;

namespace Swan.Core.Service
{
    public interface ILogService
    {
        Task AddLogAsync(LogObject log);

        Task<List<LogObject>> GetAllLogsAsync();
    }
}