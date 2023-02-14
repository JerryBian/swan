using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class BlogService : IBlogService
    {
        private readonly IFileObjectStore<BlogTagObject> _blogTagObjectStore;
        private readonly IFileObjectStore<BlogSeriesObject> _blogSeriesObjectStore;
        private readonly IFileObjectStore<BlogPostObject> _blogPostObjectStore;

        public BlogService(
            IFileObjectStore<BlogTagObject> blogTagObjectStore, 
            IFileObjectStore<BlogSeriesObject> blogSeriesObjectStore, 
            IFileObjectStore<BlogPostObject> blogPostObjectStore)
        {
            _blogPostObjectStore = blogPostObjectStore;
            _blogTagObjectStore = blogTagObjectStore;
            _blogSeriesObjectStore = blogSeriesObjectStore;
        }

        #region Posts

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            var result = new List<BlogPost>();
            var objs = await _blogPostObjectStore.GetAllAsync();
            foreach(var obj in objs)
            {
                var post = new BlogPost(obj);
                result.Add(post);
            }

            return result;
        }

        public async Task<BlogPost> GetPostAsync(string id)
        {
            var objs = await _blogPostObjectStore.GetAllAsync();
            var obj = objs.FirstOrDefault(x => x.Id == id);
            if(obj == null)
            {
                return null;
            }

            var post = new BlogPost(obj);
            return post;
        }

        public async Task<BlogPost> GetPostByLinkAsync(string link)
        {
            var objs = await _blogPostObjectStore.GetAllAsync();
            var obj = objs.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Link, link));
            if (obj == null)
            {
                return null;
            }

            var post = new BlogPost(obj);
            return post;
        }

        public async Task<BlogPostObject> CreatePostAsync(BlogPostObject obj)
        {
            return await _blogPostObjectStore.AddAsync(obj);
        }

        public async Task<BlogPostObject> UpdatePostAsync(BlogPostObject obj)
        {
            return await _blogPostObjectStore.UpdateAsync(obj);
        }

        public async Task DeletePostAsync(string id)
        {
            await _blogPostObjectStore.DeleteAsync(id);
        }

        #endregion

        #region Tags

        public async Task<BlogTag> GetTagAsync(string id)
        {
            var objs = await _blogTagObjectStore.GetAllAsync();
            var tag = objs.FirstOrDefault(x => x.Id == id);
            if(tag == null)
            {
                return null;
            }

            return new BlogTag(tag);
        }

        public async Task<BlogTag> GetTagByUrlAsync(string url)
        {
            var objs = await _blogTagObjectStore.GetAllAsync();
            var tag = objs.FirstOrDefault(x => StringHelper.EqualsIgoreCase(url, x.Url));
            if (tag == null)
            {
                return null;
            }

            return new BlogTag(tag);
        }

        public async Task<List<BlogTag>> GetAllTagsAsync()
        {
            var result = new List<BlogTag>();
            var objs = await _blogTagObjectStore.GetAllAsync();
            foreach(var obj in objs)
            {
                var tag = new BlogTag(obj);
                result.Add(tag);
            }

            return result;
        }

        public async Task<BlogTagObject> CreateTagAsync(BlogTagObject obj)
        {
            return await _blogTagObjectStore.AddAsync(obj);
        }

        public async Task<BlogTagObject> UpdateTagAsync(BlogTagObject obj)
        {
            return await _blogTagObjectStore.UpdateAsync(obj);
        }

        public async Task DeleteTagAsync(string id)
        {
            await _blogTagObjectStore.DeleteAsync(id);
        }

        #endregion

        #region Series

        public async Task<BlogSeries> GetSeriesAsync(string id)
        {
            var objs = await _blogSeriesObjectStore.GetAllAsync();
            var series = objs.FirstOrDefault(x => x.Id == id);
            if (series == null)
            {
                return null;
            }

            return new BlogSeries(series);
        }

        public async Task<BlogSeries> GetSeriesByUrlAsync(string url)
        {
            var objs = await _blogSeriesObjectStore.GetAllAsync();
            var series = objs.FirstOrDefault(x => StringHelper.EqualsIgoreCase(url, x.Url));
            if (series == null)
            {
                return null;
            }

            return new BlogSeries(series);
        }

        public async Task<List<BlogSeries>> GetAllSeriesAsync()
        {
            var result = new List<BlogSeries>();
            var objs = await _blogSeriesObjectStore.GetAllAsync();
            foreach (var obj in objs)
            {
                var series = new BlogSeries(obj);
                result.Add(series);
            }

            return result;
        }

        public async Task<BlogSeriesObject> CreateSeriesAsync(BlogSeriesObject obj)
        {
            return await _blogSeriesObjectStore.AddAsync(obj);
        }

        public async Task<BlogSeriesObject> UpdateSeriesAsync(BlogSeriesObject obj)
        {
            return await _blogSeriesObjectStore.UpdateAsync(obj);
        }

        public async Task DeleteSeriesAsync(string id)
        {
            await _blogSeriesObjectStore.DeleteAsync(id);
        }

        #endregion
    }
}
