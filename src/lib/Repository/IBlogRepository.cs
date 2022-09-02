using Laobian.Lib.Model;

namespace Laobian.Lib.Repository
{
    public interface IBlogRepository
    {
        Task<bool> AddPostAccessAsync(string id, int count, CancellationToken cancellationToken = default);

        IAsyncEnumerable<BlogPost> ReadAllPostsAsync(CancellationToken cancellationToken = default);

        Task AddPostAsync(BlogPost item, CancellationToken cancellationToken = default);

        Task UpdatePostAsync(BlogPost item, CancellationToken cancellationToken = default);
    }
}
