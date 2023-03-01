using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ILogService
    {
        Task AddLogAsync(SwanLog log);

        Task CleanupAsync();

        Task<List<SwanLog>> GetAllLogsAsync();
    }
}