using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Repository;

namespace Swan.Core.Service
{
    public class BlogService : IBlogService
    {
        private readonly IBlogTagObjectRepository _blogTagObjectRepository;
        private readonly IBlogSeriesObjectRepository _blogSeriesObjectRepository;
        private readonly IBlogPostObjectRepository _blogPostObjectRepository;

        public BlogService(
            IBlogPostObjectRepository blogPostObjectRepository, 
            IBlogTagObjectRepository blogTagObjectRepository, 
            IBlogSeriesObjectRepository blogSeriesObjectRepository)
        {
            _blogPostObjectRepository = blogPostObjectRepository;
            _blogTagObjectRepository = blogTagObjectRepository;
            _blogSeriesObjectRepository = blogSeriesObjectRepository;
        }

        #region Posts

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            var result = new List<BlogPost>();
            var objs = await _blogPostObjectRepository.GetAllAsync();
            foreach(var obj in objs)
            {
                var post = new BlogPost(obj);
                result.Add(post);
            }

            return result;
        }

        public async Task<BlogPost> GetPostAsync(string id)
        {
            var obj = await _blogPostObjectRepository.GetAsync(id);
            if(obj == null)
            {
                return null;
            }

            var post = new BlogPost(obj);
            return post;
        }

        public async Task<BlogPost> GetPostByLinkAsync(string link)
        {
            var allPosts = await _blogPostObjectRepository.GetAllAsync();
            var obj = allPosts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Link, link));
            if (obj == null)
            {
                return null;
            }

            var post = new BlogPost(obj);
            return post;
        }

        public async Task<BlogPostObject> CreatePostAsync(BlogPostObject obj)
        {
            return await _blogPostObjectRepository.CreateAsync(obj);
        }

        public async Task<BlogPostObject> UpdatePostAsync(BlogPostObject obj)
        {
            return await _blogPostObjectRepository.UpdateAsync(obj);
        }

        public async Task DeletePostAsync(string id)
        {
            await _blogPostObjectRepository.DeleteAsync(id);
        }

        #endregion

        #region Tags

        public async Task<BlogTag> GetTagAsync(string id)
        {
            var tag = await _blogTagObjectRepository.GetAsync(id);
            if(tag == null)
            {
                return null;
            }

            return new BlogTag(tag);
        }

        public async Task<BlogTag> GetTagByUrlAsync(string url)
        {
            var objs = await _blogTagObjectRepository.GetAllAsync();
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
            var objs = await _blogTagObjectRepository.GetAllAsync();
            foreach(var obj in objs)
            {
                var tag = new BlogTag(obj);
                result.Add(tag);
            }

            return result;
        }

        public async Task<BlogTagObject> CreateTagAsync(BlogTagObject obj)
        {
            return await _blogTagObjectRepository.CreateAsync(obj);
        }

        public async Task<BlogTagObject> UpdateTagAsync(BlogTagObject obj)
        {
            return await _blogTagObjectRepository.UpdateAsync(obj);
        }

        public async Task DeleteTagAsync(string id)
        {
            await _blogTagObjectRepository.DeleteAsync(id);
        }

        #endregion

        #region Series

        public async Task<BlogSeries> GetSeriesAsync(string id)
        {
            var series = await _blogSeriesObjectRepository.GetAsync(id);
            if (series == null)
            {
                return null;
            }

            return new BlogSeries(series);
        }

        public async Task<BlogSeries> GetSeriesByUrlAsync(string url)
        {
            var objs = await _blogSeriesObjectRepository.GetAllAsync();
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
            var objs = await _blogSeriesObjectRepository.GetAllAsync();
            foreach (var obj in objs)
            {
                var series = new BlogSeries(obj);
                result.Add(series);
            }

            return result;
        }

        public async Task<BlogSeriesObject> CreateSeriesAsync(BlogSeriesObject obj)
        {
            return await _blogSeriesObjectRepository.CreateAsync(obj);
        }

        public async Task<BlogSeriesObject> UpdateSeriesAsync(BlogSeriesObject obj)
        {
            return await _blogSeriesObjectRepository.UpdateAsync(obj);
        }

        public async Task DeleteSeriesAsync(string id)
        {
            await _blogSeriesObjectRepository.DeleteAsync(id);
        }

        #endregion
    }
}
