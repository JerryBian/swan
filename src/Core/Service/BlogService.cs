using HtmlAgilityPack;
using Swan.Core.Cache;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class BlogService : IBlogService
    {
        private readonly IMemoryObjectStore _store;

        public BlogService(IMemoryObjectStore store)
        {
            _store = store;
        }

        #region Posts

        public async Task<List<BlogPost>> GetAllPostsAsync(bool isAdmin)
        {
            var posts = await _store.GetBlogPostsAsync(isAdmin);
            return posts.OrderByDescending(x => x.Object.PublishTime).ToList();
        }

        public async Task<BlogPost> GetPostAsync(string id)
        {
            var posts = await _store.GetBlogPostsAsync(true);
            return posts.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<BlogPost> GetPostByLinkAsync(string link,bool isAdmin)
        {
            var allPosts = await _store.GetBlogPostsAsync(isAdmin);
            return allPosts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(link, x.Object.Link));
        }

        public async Task<BlogPost> AddPostAsync(BlogPostObject obj)
        {
            var result = await _store.AddPostAsync(obj);
            return result;
        }

        public async Task<BlogPost> UpdatePostAsync(BlogPostObject obj)
        {
            var result = await _store.UpdatePostAsync(obj);
            return result;
        }

        #endregion

        #region Tags

        public async Task<BlogTag> GetTagAsync(string id)
        {
            var tags = await _store.GetBlogTagsAsync(true);
            return tags.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<BlogTag> GetTagByUrlAsync(string url, bool isAdmin)
        {
            var tags = await _store.GetBlogTagsAsync(isAdmin);
            return tags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Object.Url, url));
        }

        public async Task<List<BlogTag>> GetAllTagsAsync(bool isAdmin)
        {
            var tags = await _store.GetBlogTagsAsync(isAdmin);
            return tags.OrderByDescending(x => x.Posts.Any() ? x.Posts.Max(y => y.Object.PublishTime) : default).ToList();
        }

        public async Task<BlogTag> AddTagAsync(BlogTagObject obj)
        {
            var result = await _store.AddTagAsync(obj);
            return result;
        }

        public async Task<BlogTag> UpdateTagAsync(BlogTagObject obj)
        {
            var result = await _store.UpdateTagAsync(obj);
            return result;
        }

        public async Task DeleteTagAsync(string id)
        {
            await _store.DeleteTagAsync(id);
        }

        #endregion

        #region Series

        public async Task<BlogSeries> GetSeriesAsync(string id)
        {
            var allSeries = await _store.GetBlogSeriesAsync(true);
            return allSeries.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<BlogSeries> GetSeriesByUrlAsync(string url, bool isAdmin)
        {
            var allSeries = await _store.GetBlogSeriesAsync(isAdmin);
            return allSeries.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Object.Url, url));
        }

        public async Task<List<BlogSeries>> GetAllSeriesAsync(bool isAdmin)
        {
            var result = await _store.GetBlogSeriesAsync(isAdmin);
            return result.OrderByDescending(x => x.Posts.Any() ? x.Posts.Max(y => y.Object.PublishTime) : default).ToList();
        }

        public async Task<BlogSeries> AddSeriesAsync(BlogSeriesObject obj)
        {
            var result = await _store.AddSeriesAsync(obj);
            return result;
        }

        public async Task<BlogSeries> UpdateSeriesAsync(BlogSeriesObject obj)
        {
            var result = await _store.UpdateSeriesAsync(obj);
            return result;
        }

        public async Task DeleteSeriesAsync(string id)
        {
            await _store.DeleteSeriesAsync(id);
        }

        #endregion
    }
}
