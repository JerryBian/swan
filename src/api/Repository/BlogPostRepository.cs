using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.SourceProvider;
using Laobian.Share.Blog;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ISourceProvider _sourceProvider;
        private List<BlogPost> _blogPosts;

        public BlogPostRepository(ISourceProviderFactory sourceProviderFactory, IOptions<ApiConfig> apiConfig)
        {
            _blogPosts = new List<BlogPost>();
            _sourceProvider = sourceProviderFactory.Get(apiConfig.Value.Source);
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.LoadAsync(cancellationToken);
            var posts = await _sourceProvider.GetPostsAsync(cancellationToken);
            _blogPosts = new List<BlogPost>();
            foreach (var post in posts)
            {
                _blogPosts.Add(new BlogPost
                {
                    Link = post.Key,
                    MdContent = post.Value
                });
            }
        }

        public async Task<List<BlogPost>> GetPostsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPosts);
        }
    }
}