using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Misc;

namespace Laobian.Api.Service;

public interface IGitFileService
{
    Task PullAsync(CancellationToken cancellationToken = default);

    Task PushAsync(string message, CancellationToken cancellationToken = default);

    Task<List<GitFileStat>> GetGitFileStatsAsync(CancellationToken cancellationToken = default);
}