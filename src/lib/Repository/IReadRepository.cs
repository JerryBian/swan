using Laobian.Lib.Model;

namespace Laobian.Lib.Repository
{
    public interface IReadRepository
    {
        IAsyncEnumerable<ReadItem> ReadAllAsync(CancellationToken cancellationToken = default);

        Task AddAsync(ReadItem item, CancellationToken cancellationToken = default);

        Task UpdateAsync(ReadItem item, CancellationToken cancellationToken = default);
    }
}
