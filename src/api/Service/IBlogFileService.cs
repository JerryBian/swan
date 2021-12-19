using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Site.Blog;

namespace Laobian.Api.Service
{
    public interface IBlogFileService
    {
        #region Blog

        Task<List<BlogPost>> GetBlogPostsAsync(CancellationToken cancellationToken = default);

        Task<BlogPost> GetBlogPostAsync(string postLink, CancellationToken cancellationToken = default);

        Task AddBlogPostAsync(BlogPost blogPost, CancellationToken cancellationToken = default);

        Task UpdateBlogPostAsync(BlogPost blogPost, string originalPostLink, CancellationToken cancellationToken = default);

        Task DeleteBlogPostAsync(string postLink, CancellationToken cancellationToken = default);

        Task<List<BlogAccess>> GetBlogPostAccessAsync(string postLink,
            CancellationToken cancellationToken = default);

        Task AddBlogPostAccessAsync(string postLink, DateTime date, int count,
            CancellationToken cancellationToken = default);

        Task<List<BlogTag>> GetBlogTagsAsync(CancellationToken cancellationToken = default);

        Task<BlogTag> GetBlogTagAsync(string id, CancellationToken cancellationToken = default);

        Task AddBlogTagAsync(BlogTag blogTag, CancellationToken cancellationToken = default);

        Task UpdateBlogTagAsync(BlogTag blogTag, CancellationToken cancellationToken = default);

        Task DeleteBlogTagAsync(string id, CancellationToken cancellationToken = default);

        #endregion
    }
}
