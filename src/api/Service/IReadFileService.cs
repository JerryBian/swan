using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Model.Read;

namespace Laobian.Api.Service;

public interface IReadFileService
{
    Task<List<ReadItem>> GetReadItemsAsync(CancellationToken cancellationToken = default);

    Task<List<ReadItem>> GetReadItemsAsync(int year, CancellationToken cancellationToken = default);

    Task AddReadItemAsync(ReadItem readItem, CancellationToken cancellationToken = default);

    Task UpdateReadItemAsync(ReadItem readItem, CancellationToken cancellationToken = default);

    Task DeleteReadItemAsync(string id,
        CancellationToken cancellationToken = default);
}