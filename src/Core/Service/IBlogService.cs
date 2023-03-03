using Swan.Core.Model;
using Swan.Core.Model.Object;

namespace Swan.Core.Service
{
    public interface IBlogService
    {
        Task<BlogPost> AddPostAsync(BlogPostObject obj);

        Task<BlogSeries> AddSeriesAsync(BlogSeriesObject obj);

        Task<BlogTag> AddTagAsync(BlogTagObject obj);

        Task DeleteSeriesAsync(string id);

        Task DeleteTagAsync(string id);

        Task<List<BlogPost>> GetAllPostsAsync(bool isAdmin);

        Task<List<BlogSeries>> GetAllSeriesAsync(bool isAdmin);

        Task<List<BlogTag>> GetAllTagsAsync(bool isAdmin);

        Task<BlogPost> GetPostAsync(string id);

        Task<BlogPost> GetPostByLinkAsync(string link, bool isAdmin);

        Task<BlogSeries> GetSeriesAsync(string id);

        Task<BlogSeries> GetSeriesByUrlAsync(string url, bool isAdmin);

        Task<BlogTag> GetTagAsync(string id);

        Task<BlogTag> GetTagByUrlAsync(string url, bool isAdmin);

        Task<BlogPost> UpdatePostAsync(BlogPostObject obj);

        Task<BlogSeries> UpdateSeriesAsync(BlogSeriesObject obj);

        Task<BlogTag> UpdateTagAsync(BlogTagObject obj);
    }
}