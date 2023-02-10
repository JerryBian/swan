using Swan.Core.Model;
using Swan.Core.Model.Object;

namespace Swan.Core.Service
{
    public interface IBlogService
    {
        Task<List<BlogPost>> GetAllPostsAsync();

        Task<BlogTag> GetTagAsync(string id);

        Task<BlogTag> GetTagByUrlAsync(string url);

        Task<List<BlogTag>> GetAllTagsAsync();

        Task<BlogTagObject> CreateTagAsync(BlogTagObject obj);

        Task<BlogTagObject> UpdateTagAsync(BlogTagObject obj);

        Task DeleteTagAsync(string id);

        Task<BlogSeries> GetSeriesAsync(string id);

        Task<BlogSeries> GetSeriesByUrlAsync(string url);

        Task<List<BlogSeries>> GetAllSeriesAsync();

        Task<BlogSeriesObject> CreateSeriesAsync(BlogSeriesObject obj);

        Task<BlogSeriesObject> UpdateSeriesAsync(BlogSeriesObject obj);

        Task DeleteSeriesAsync(string id);
    }
}
