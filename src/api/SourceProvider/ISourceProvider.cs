using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Api.SourceProvider
{
    public interface ISourceProvider
    {
        Task LoadAsync(bool init = true, CancellationToken cancellationToken = default);

        Task<IDictionary<string, string>> GetPostsAsync(CancellationToken cancellationToken = default);

        Task<string> GetTagsAsync(CancellationToken cancellationToken = default);

        Task SaveTagsAsync(string tags, CancellationToken cancellationToken = default);

        Task<string> GetPostMetadataAsync(CancellationToken cancellationToken = default);

        Task SavePostMetadataAsync(string metadata, CancellationToken cancellationToken = default);

        Task<IDictionary<string, string>> GetPostAccessAsync(CancellationToken cancellationToken = default);

        Task SavePostAccessAsync(IDictionary<string, string> postAccess, CancellationToken cancellationToken = default);

        Task PersistentAsync(CancellationToken cancellationToken = default);
    }
}