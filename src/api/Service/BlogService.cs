using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Api.Store;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Service
{
    public class BlogService
    {
        private BlogTagStore _blogTagStore;
        private BlogPostStore _blogPostStore;

        private readonly IDbRepository _dbRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogService(IDbRepository dbRepository, IBlogPostRepository blogPostRepository)
        {
            _dbRepository = dbRepository;
            _blogPostRepository = blogPostRepository;
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            _blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            _blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
        }

        public async Task AddBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            _blogTagStore.Add(tag);
            await Task.CompletedTask;
        }

        public async Task UpdateBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            _blogTagStore.Update(tag);
            await Task.CompletedTask;
        }

        public async Task RemoveBlogTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var allPosts = _blogPostStore.GetAll();
            allPosts.ForEach(x => x.Tags.RemoveAll(y => StringHelper.EqualIgnoreCase(y.Link, tagLink)));
            _blogTagStore.RemoveByLink(tagLink);
            await Task.CompletedTask;
        }
    }
}
