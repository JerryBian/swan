using Laobian.Lib.Cache;
using Laobian.Lib.Extension;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Repository;

namespace Laobian.Lib.Service
{
    public class BlogService : IBlogService
    {
        private const string PostCacheKey = "AllBlogPosts";

        private readonly ICacheManager _cacheManager;
        private readonly IBlogRepository _blogRepository;

        public BlogService(ICacheManager cacheManager, IBlogRepository readRepository)
        {
            _cacheManager = cacheManager;
            _blogRepository = readRepository;
        }

        public async Task<List<BlogPostView>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            return await _cacheManager.GetOrCreateAsync(PostCacheKey, async () =>
            {
                List<BlogPostView> result = new();
                await foreach (BlogPost item in _blogRepository.ReadAllPostsAsync(cancellationToken))
                {
                    BlogPostView view = new(item)
                    {
                        HtmlContent = MarkdownHelper.ToHtml(item.MdContent),
                        FullLink = $"/blog/{item.PublishTime:yyyy/MM}/{item.Link.ToLowerInvariant()}.html",
                        Metadata = $"{item.PublishTime.ToCnDate()} &middot; {item.AccessCount} 次阅读"
                    };
                    result.Add(view);
                }

                return result;
            });
        }

        public async Task<BlogPostView> GetPostAsync(string id, CancellationToken cancellationToken = default)
        {
            List<BlogPostView> items = await GetAllPostsAsync(cancellationToken);
            return items.FirstOrDefault(x => x.Raw.Id == id);
        }

        public async Task<BlogPostView> GetPostAsync(int year, int month, string link, CancellationToken cancellationToken = default)
        {
            List<BlogPostView> items = await GetAllPostsAsync(cancellationToken);
            return items.FirstOrDefault(x => x.Raw.PublishTime.Year == year && x.Raw.PublishTime.Month == month && string.Equals(x.Raw.Link, link, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<BlogPostView> AddPostAsync(BlogPost item, CancellationToken cancellationToken = default)
        {
            await _blogRepository.AddPostAsync(item, cancellationToken);
            ClearCache();
            return await GetPostAsync(item.Id, cancellationToken);
        }

        public async Task<BlogPostView> UpdateAsync(BlogPost item, CancellationToken cancellationToken = default)
        {
            await _blogRepository.UpdatePostAsync(item, cancellationToken);
            ClearCache();
            return await GetPostAsync(item.Id, cancellationToken);
        }

        public async Task<bool> AddPostAccessAsync(string id, int count, CancellationToken cancellationToken = default)
        {
            return await _blogRepository.AddPostAccessAsync(id, count, cancellationToken);
        }

        private void ClearCache()
        {
            _cacheManager.TryRemove(PostCacheKey);
        }
    }
}
