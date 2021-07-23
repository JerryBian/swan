using System;
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
        private Lazy<BlogPostStore> _blogPostStore;

        public BlogPostRepository(ISourceProviderFactory sourceProviderFactory, IOptions<ApiConfig> apiConfig)
        {
            _sourceProvider = sourceProviderFactory.Get(apiConfig.Value.Source);
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.LoadAsync(false, cancellationToken);
            _blogPostStore = new Lazy<BlogPostStore>(() => // Ugly
            {
                var posts = _sourceProvider.GetPostsAsync(cancellationToken).Result;
                var postStore = new BlogPostStore(posts);
                return postStore;
            }, true);
        }

        public async Task<BlogPostStore> GetBlogPostStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPostStore.Value);
        }
    }
}