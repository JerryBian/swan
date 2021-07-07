using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.SourceProvider;
using Laobian.Api.Store;
using Laobian.Share.Blog;
using Laobian.Share.Helper;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class DbRepository : IDbRepository
    {
        private readonly ISourceProvider _sourceProvider;
        private IDictionary<Guid, BlogCommentItem> _blogPostCommentItems;
        private IDictionary<string, List<BlogCommentItem>> _blogPostComments;
        private IDictionary<string, IDictionary<DateTime, int>> _blogPostsAccess;
        private IDictionary<string, BlogPostMetadata> _blogPostsMetadata;
        private List<BlogTag> _blogTags;

        private BlogTagStore _blogTagStore;
        private BlogAccessStore _blogAccessStore;
        private BlogCommentStore _blogCommentStore;
        private BlogMetadataStore _blogMetadataStore;


        public DbRepository(ISourceProviderFactory sourceProviderFactory, IOptions<ApiConfig> apiConfig)
        {
            _sourceProvider = sourceProviderFactory.Get(apiConfig.Value.Source);
            _blogTags = new List<BlogTag>();
            _blogPostsMetadata = new ConcurrentDictionary<string, BlogPostMetadata>();
            _blogPostCommentItems = new ConcurrentDictionary<Guid, BlogCommentItem>();
            _blogPostComments = new ConcurrentDictionary<string, List<BlogCommentItem>>();
            _blogPostsAccess = new ConcurrentDictionary<string, IDictionary<DateTime, int>>();
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.LoadAsync(cancellationToken);

            var tags = await _sourceProvider.GetTagsAsync(cancellationToken);
            _blogTagStore = new BlogTagStore(tags);

            var postMetadata = await _sourceProvider.GetPostMetadataAsync(cancellationToken);
            _blogMetadataStore = new BlogMetadataStore(postMetadata);
            
            var postComments = await _sourceProvider.GetCommentsAsync(cancellationToken);
            _blogCommentStore = new BlogCommentStore(postComments);

            var postAccess = await _sourceProvider.GetPostAccessAsync(cancellationToken);
            _blogAccessStore = new BlogAccessStore(postAccess);
        }

        public async Task<BlogTagStore> GetBlogTagStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogTagStore);
        }

        public async Task PersistentBlogTagStoreAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SaveTagsAsync(JsonHelper.Serialize(_blogTagStore.GetAll().OrderByDescending(x =>x.LastUpdatedAt), true), cancellationToken);
        }

        public async Task<BlogMetadataStore> GetBlogMetadataStoreAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(_blogMetadataStore);
        }

        public async Task PersistentBlogMetadataAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SavePostMetadataAsync(
                JsonHelper.Serialize(_blogMetadataStore.GetAll().OrderByDescending(x => x.LastUpdateTime), true),
                cancellationToken);
        }

        public async Task<BlogAccessStore> GetBlogAccessStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogAccessStore);
        }

        public async Task PersistentBlogAccessStoreAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SavePostAccessAsync(_blogAccessStore.GetAll().ToDictionary(x => x.Key, x => JsonHelper.Serialize(x.Value.OrderByDescending(y => y.Date), true)), cancellationToken);
        }

        public async Task<BlogCommentStore> GetBlogCommentStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogCommentStore);
        }

        public async Task PersistentBlogCommentStoreAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SaveCommentsAsync(_blogCommentStore.GetAll().ToDictionary(x => x.Key, x => JsonHelper.Serialize(x.Value.OrderByDescending(y => y.LastUpdatedAt), true)), cancellationToken);
        }

        public async Task<List<BlogTag>> GetBlogTagsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogTags);
        }

        public async Task SaveBlogTagsAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SaveTagsAsync(JsonHelper.Serialize(_blogTags, true), cancellationToken);
        }

        public async Task<IDictionary<string, BlogPostMetadata>> GetBlogPostMetadataAsync(
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPostsMetadata);
        }

        public async Task SaveBlogPostMetadataAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SavePostMetadataAsync(JsonHelper.Serialize(_blogPostsMetadata.Values, true),
                cancellationToken);
        }

        public async Task<IDictionary<string, IDictionary<DateTime, int>>> GetBlogPostAccessAsync(
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPostsAccess);
        }

        public async Task SaveBlogPostAccessAsync(CancellationToken cancellationToken = default)
        {
            var access = new Dictionary<string, string>();
            foreach (var item in _blogPostsAccess)
            {
                var postAccess = new Dictionary<string, int>();
                foreach (var i in item.Value)
                {
                    postAccess.Add(i.Key.ToString("yyyy-MM-dd"), i.Value);
                }

                access.Add(item.Key, JsonHelper.Serialize(postAccess));
            }

            await _sourceProvider.SavePostAccessAsync(access, cancellationToken);
        }

        public async Task<IDictionary<string, List<BlogCommentItem>>> GetBlogPostCommentsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPostComments);
        }

        public async Task SaveBlogPostCommentsAsync(CancellationToken cancellationToken = default)
        {
            var comments = new Dictionary<string, string>();
            foreach (var blogPostComment in _blogPostComments)
            {
                comments.Add(blogPostComment.Key, JsonHelper.Serialize(blogPostComment.Value));
            }

            await _sourceProvider.SaveCommentsAsync(comments, cancellationToken);
        }

        public async Task<IDictionary<Guid, BlogCommentItem>> GetBlogPostCommentItemsAsync(
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogPostCommentItems);
        }
    }
}