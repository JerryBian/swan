using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.SourceProvider;
using Laobian.Api.Store;
using Laobian.Share.Blog;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ISourceProvider _sourceProvider;
        private BlogPostStore _blogPostStore;

        public BlogPostRepository(ISourceProviderFactory sourceProviderFactory, IOptions<ApiConfig> apiConfig)
        {
            _sourceProvider = sourceProviderFactory.Get(apiConfig.Value.Source);
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.LoadAsync(cancellationToken);
            var posts = await _sourceProvider.GetPostsAsync(cancellationToken);
            _blogPostStore = new BlogPostStore(posts);
        }

        public async Task<BlogPostStore> GetBlogPostStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPostStore);
        }
    }
}