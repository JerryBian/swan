using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Api.Service
{
    public interface IBlogService
    {
        Task LoadAsync(CancellationToken cancellationToken = default);

        Task PersistentAsync(string message, CancellationToken cancellationToken = default);

        Task<List<BlogPost>> GetAllPostsAsync(CancellationToken cancellationToken = default);

        Task<List<BlogTag>> GetAllTagsAsync(CancellationToken cancellationToken = default);

        Task<BlogPost> GetPostAsync(string postLink, CancellationToken cancellationToken = default);

        Task<BlogTag> GetTagAsync(string tagLink, CancellationToken cancellationToken = default);

        Task AddBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default);

        Task UpdateBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default);

        Task RemoveBlogTagAsync(string tagLink, CancellationToken cancellationToken = default);

        Task UpdateBlogPostMetadataAsync(BlogPostMetadata metadata, CancellationToken cancellationToken = default);

        Task AddBlogAccessAsync(string postLink, CancellationToken cancellationToken = default);
    }
}