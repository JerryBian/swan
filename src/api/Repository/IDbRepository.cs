using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Store;
using Laobian.Share.Blog;

namespace Laobian.Api.Repository
{
    public interface IDbRepository
    {
        Task LoadAsync(CancellationToken cancellationToken = default);

        Task<BlogTagStore> GetBlogTagStoreAsync(CancellationToken cancellationToken = default);

        Task PersistentBlogTagStoreAsync(CancellationToken cancellationToken = default);

        Task<List<BlogTag>> GetBlogTagsAsync(CancellationToken cancellationToken = default);

        Task SaveBlogTagsAsync(CancellationToken cancellationToken = default);

        Task<IDictionary<string, BlogPostMetadata>> GetBlogPostMetadataAsync(
            CancellationToken cancellationToken = default);

        Task SaveBlogPostMetadataAsync(CancellationToken cancellationToken = default);

        Task<IDictionary<string, IDictionary<DateTime, int>>> GetBlogPostAccessAsync(
            CancellationToken cancellationToken = default);

        Task SaveBlogPostAccessAsync(CancellationToken cancellationToken = default);

        Task<IDictionary<string, List<BlogCommentItem>>> GetBlogPostCommentsAsync(
            CancellationToken cancellationToken = default);

        Task SaveBlogPostCommentsAsync(CancellationToken cancellationToken = default);

        Task<IDictionary<Guid, BlogCommentItem>> GetBlogPostCommentItemsAsync(
            CancellationToken cancellationToken = default);
    }
}