using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Store;
using Laobian.Share.Blog;

namespace Laobian.Api.Repository
{
    public interface IBlogPostRepository
    {
        Task LoadAsync(CancellationToken cancellationToken = default);

        Task<BlogPostStore> GetBlogPostStoreAsync(CancellationToken cancellationToken = default);
    }
}