﻿using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Store;

namespace Laobian.Api.Repository
{
    public interface IDbRepository
    {
        Task LoadAsync(CancellationToken cancellationToken = default);

        Task<BlogTagStore> GetBlogTagStoreAsync(CancellationToken cancellationToken = default);

        Task PersistentBlogTagStoreAsync(CancellationToken cancellationToken = default);

        Task<BlogMetadataStore> GetBlogMetadataStoreAsync(CancellationToken cancellationToken);

        Task PersistentBlogMetadataAsync(CancellationToken cancellationToken = default);

        Task<BlogAccessStore> GetBlogAccessStoreAsync(CancellationToken cancellationToken = default);

        Task PersistentBlogAccessStoreAsync(CancellationToken cancellationToken = default);

        Task<BlogCommentStore> GetBlogCommentStoreAsync(CancellationToken cancellationToken = default);

        Task PersistentBlogCommentStoreAsync(CancellationToken cancellationToken = default);
    }
}