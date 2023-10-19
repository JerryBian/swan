using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ISwanService
    {
        Task AddBlogPostAsync(BlogPost blogPost);
        Task AddBlogSeriesAsync(BlogSeries blogSeries);
        Task AddBlogTagAsync(BlogTag blogTag);
        Task AddReadItemAsync(ReadItem readItem);
        Task DeleteBlogSeriesAsync(string id);
        Task DeleteBlogTagAsync(string id);
        Task DeleteReadItemAsync(string id);
        Task<List<BlogPost>> GetBlogPostsAsync();
        Task<List<BlogSeries>> GetBlogSeriesAsync();
        Task<List<BlogTag>> GetBlogTagsAsync();
        Task<List<ReadItem>> GetReadItemsAsync();
        Task UpdateBlogPostAsync(BlogPost blogPost);
        Task UpdateBlogSeriesAsync(BlogSeries blogSeries);
        Task UpdateBlogTagAsync(BlogTag blogTag);
        Task UpdateReadItemAsync(ReadItem readItem);
    }
}