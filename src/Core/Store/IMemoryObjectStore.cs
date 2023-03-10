using Swan.Core.Model;
using Swan.Core.Model.Object;

namespace Swan.Core.Store
{
    public interface IMemoryObjectStore
    {
        Task<BlogPost> AddPostAsync(BlogPostObject obj);

        Task<ReadModel> AddReadAsync(ReadObject obj);

        Task<BlogSeries> AddSeriesAsync(BlogSeriesObject obj);

        Task<BlogTag> AddTagAsync(BlogTagObject obj);

        Task DeleteReadAsync(string id);

        Task DeleteSeriesAsync(string id);

        Task DeleteTagAsync(string id);

        Task<List<BlogPost>> GetBlogPostsAsync(bool isAdmin);

        Task<List<BlogSeries>> GetBlogSeriesAsync(bool isAdmin);

        Task<List<BlogTag>> GetBlogTagsAsync(bool isAdmin);

        Task<List<ReadModel>> GetReadModelsAsync(bool isAdmin);

        Task<BlogPost> UpdatePostAsync(BlogPostObject obj, bool coreUpdate);

        Task<ReadModel> UpdateReadAsync(ReadObject obj);

        Task<BlogSeries> UpdateSeriesAsync(BlogSeriesObject obj);

        Task<BlogTag> UpdateTagAsync(BlogTagObject obj);
    }
}