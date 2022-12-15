using Swan.Lib.Model;

namespace Swan.Lib.Service
{
    public interface IBlogService
    {
        Task<List<BlogPostView>> GetAllPostsAsync(CancellationToken cancellationToken = default);

        Task<BlogPostView> GetPostAsync(string id, CancellationToken cancellationToken = default);

        Task<BlogPostView> GetPostAsync(int year, int month, string link, CancellationToken cancellationToken = default);

        Task<BlogPostView> AddPostAsync(BlogPost item, CancellationToken cancellationToken = default);

        Task<BlogPostView> UpdateAsync(BlogPost item, CancellationToken cancellationToken = default);

        Task<bool> AddPostAccessAsync(string id, int count, CancellationToken cancellationToken = default);
    }
}
