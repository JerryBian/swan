using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ISwanLogService
    {
        Task<List<SwanLog>> GetAsync(CancellationToken cancellationToken = default);

        Task AddAsync(SwanLog log);
    }
}
