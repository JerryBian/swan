using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Store;

namespace Laobian.Api.Repository
{
    public interface IDbRepository
    {
        Task LoadAsync(CancellationToken cancellationToken = default);

        Task<BlogTagStore> GetBlogTagStoreAsync(CancellationToken cancellationToken = default);

        Task<BlogMetadataStore> GetBlogMetadataStoreAsync(CancellationToken cancellationToken);

        Task<BlogAccessStore> GetBlogAccessStoreAsync(CancellationToken cancellationToken = default);

        Task PersistentAsync(string message, CancellationToken cancellationToken = default);

        Task<ReadItemStore> GetReadItemsStoreAsync(CancellationToken cancellationToken = default);
    }
}