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

        private BlogTagStore _blogTagStore;
        private BlogAccessStore _blogAccessStore;
        private BlogCommentStore _blogCommentStore;
        private BlogMetadataStore _blogMetadataStore;


        public DbRepository(ISourceProviderFactory sourceProviderFactory, IOptions<ApiConfig> apiConfig)
        {
            _sourceProvider = sourceProviderFactory.Get(apiConfig.Value.Source);
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
    }
}