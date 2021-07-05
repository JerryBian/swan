using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.SourceProvider;
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
            if (!string.IsNullOrEmpty(tags))
            {
                _blogTags = JsonHelper.Deserialize<List<BlogTag>>(tags);
            }

            var postMetadata = await _sourceProvider.GetPostMetadataAsync(cancellationToken);
            if (!string.IsNullOrEmpty(postMetadata))
            {
                var metadata = JsonHelper.Deserialize<List<BlogPostMetadata>>(postMetadata);
                _blogPostsMetadata =
                    new ConcurrentDictionary<string, BlogPostMetadata>(metadata.ToDictionary(x => x.Link),
                        StringComparer.InvariantCultureIgnoreCase);
            }

            var postComments = await _sourceProvider.GetCommentsAsync(cancellationToken);
            if (postComments != null && postComments.Any())
            {
                _blogPostCommentItems = new ConcurrentDictionary<Guid, BlogCommentItem>();
                _blogPostComments =
                    new ConcurrentDictionary<string, List<BlogCommentItem>>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var postComment in postComments)
                {
                    var commentItems = JsonHelper.Deserialize<List<BlogCommentItem>>(postComment.Value);
                    _blogPostComments.TryAdd(postComment.Key, commentItems);
                    foreach (var blogCommentItem in commentItems)
                    {
                        _blogPostCommentItems.TryAdd(blogCommentItem.Id, blogCommentItem);
                    }
                }
            }

            var postAccess = await _sourceProvider.GetPostAccessAsync(cancellationToken);
            if (postAccess != null && postAccess.Any())
            {
                _blogPostsAccess =
                    new ConcurrentDictionary<string, IDictionary<DateTime, int>>(StringComparer
                        .InvariantCultureIgnoreCase);
                foreach (var item in postAccess)
                {
                    var access = JsonHelper.Deserialize<ConcurrentDictionary<DateTime, int>>(item.Value);
                    _blogPostsAccess.TryAdd(item.Key, access);
                }
            }
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