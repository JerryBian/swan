using Swan.Lib.Model;

namespace Swan.Lib.Service
{
    public interface IReadService
    {
        Task<List<ReadItemView>> GetAllAsync(CancellationToken cancellationToken = default);

        Task AddAsync(ReadItem item, CancellationToken cancellationToken = default);

        Task UpdateAsync(ReadItem item, CancellationToken cancellationToken = default);

        Task<ReadItemView> GetAsync(string id, CancellationToken cancellationToken = default);
    }
}
