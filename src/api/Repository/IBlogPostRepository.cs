using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Api.Repository
{
    public interface IBlogPostRepository
    {
        Task LoadAsync(CancellationToken cancellationToken = default);

        Task<List<BlogPost>> GetPostsAsync(CancellationToken cancellationToken = default);
    }
}