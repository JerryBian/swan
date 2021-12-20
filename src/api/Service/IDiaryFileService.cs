using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Site.Jarvis;

namespace Laobian.Api.Service
{
    public interface IDiaryFileService
    {
        Task<List<Diary>> GetDiariesAsync(int offset = 0, int? count = null, int? year = null, int? month = null, CancellationToken cancellationToken = default);

        Task<int> GetDiaryCountAsync(int? year = null, int? month = null, CancellationToken cancellationToken = default);

        Task<Diary> GetDiaryAsync(DateTime date, CancellationToken cancellationToken = default);

        Task AddDiaryAsync(Diary diary, CancellationToken cancellationToken = default);

        Task UpdateDiaryAsync(Diary diary, CancellationToken cancellationToken = default);
    }
}
