using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ISwanService
    {
        Task AddBlogPostAsync(BlogPost blogPost);

        Task AddBlogSeriesAsync(BlogSeries blogSeries);

        Task AddBlogTagAsync(BlogTag blogTag);

        Task DeleteBlogSeriesAsync(string id);

        Task DeleteBlogTagAsync(string id);

        Task<List<BlogPost>> GetBlogPostsAsync();

        Task<List<BlogSeries>> GetBlogSeriesAsync();

        Task<List<BlogTag>> GetBlogTagsAsync();

        Task UpdateBlogPostAsync(BlogPost blogPost);

        Task UpdateBlogSeriesAsync(BlogSeries blogSeries);

        Task UpdateBlogTagAsync(BlogTag blogTag);
    }
}