using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.HttpService;
using Laobian.Api.Repository;
using Laobian.Share.Blog;
using Laobian.Share.Util;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Service
{
    public class BlogService : IBlogService
    {
        private readonly BlogHttpService _blogHttpService;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IDbRepository _dbRepository;
        private readonly ApiOption _option;
        private readonly SystemLocker _systemLocker;

        public BlogService(IOptions<ApiOption> config, IDbRepository dbRepository,
            IBlogPostRepository blogPostRepository, BlogHttpService blogHttpService, SystemLocker systemLocker)
        {
            _option = config.Value;
            _systemLocker = systemLocker;
            _dbRepository = dbRepository;
            _blogHttpService = blogHttpService;
            _blogPostRepository = blogPostRepository;
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            _systemLocker.Acquire();
            await Task.WhenAll(_blogPostRepository.LoadAsync(cancellationToken),
                _dbRepository.LoadAsync(cancellationToken));
            await AggregateStoreAsync(cancellationToken);
        }

        public async Task PersistentAsync(string message, CancellationToken cancellationToken = default)
        {
            try
            {
                _systemLocker.Pause();
                await _dbRepository.PersistentAsync(message, cancellationToken);
            }
            finally
            {
                _systemLocker.Resume();
            }

            await LoadAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            _systemLocker.Acquire();
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var allPosts = blogPostStore.GetAll();
            foreach (var blogPost in allPosts)
            {
                await SetPostRawData(blogPost, cancellationToken);
            }

            return allPosts;
        }

        public async Task<List<BlogTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
        {
            _systemLocker.Acquire();
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            var allBlogs = blogTagStore.GetAll();
            return allBlogs;
        }

        public async Task<BlogPost> GetPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            _systemLocker.Acquire();
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var post = blogPostStore.GetByLink(postLink);
            if (post != null)
            {
                await SetPostRawData(post, cancellationToken);
            }

            return post;
        }

        public async Task<BlogTag> GetTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            _systemLocker.Acquire();
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            return blogTagStore.GetByLink(tagLink);
        }

        public async Task AddBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            try
            {
                _systemLocker.Pause();
                var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
                blogTagStore.Add(tag);
            }
            finally
            {
                _systemLocker.Resume();
            }
        }

        public async Task UpdateBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.Update(tag);
            await _blogHttpService.ReloadBlogDataAsync();
        }

        public async Task RemoveBlogTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var allPosts = blogPostStore.GetAll();
            allPosts.ForEach(x => x.Tags.RemoveAll(y => StringUtil.EqualsIgnoreCase(y.Link, tagLink)));

            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.RemoveByLink(tagLink);
            await _blogHttpService.ReloadBlogDataAsync();
        }

        public async Task UpdateBlogPostMetadataAsync(BlogMetadata metadata,
            CancellationToken cancellationToken = default)
        {
            var blogMetadataStore = await _dbRepository.GetBlogMetadataStoreAsync(cancellationToken);
            blogMetadataStore.Update(metadata);
            await _blogHttpService.ReloadBlogDataAsync();
        }

        public async Task AddBlogAccessAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var blogAccessStore = await _dbRepository.GetBlogAccessStoreAsync(cancellationToken);
            blogAccessStore.Add(postLink, DateTime.Now.Date, 1);
        }

        private async Task SetPostRawData(BlogPost blogPost, CancellationToken cancellationToken)
        {
            var blogAccessStore = await _dbRepository.GetBlogAccessStoreAsync(cancellationToken);
            var access = blogAccessStore.GetByLink(blogPost.Link);
            if (access != null)
            {
                blogPost.Accesses.Clear();
                blogPost.Accesses.AddRange(access);
            }

            var blogMetadataStore = await _dbRepository.GetBlogMetadataStoreAsync(cancellationToken);
            var metadata = blogMetadataStore.GetByLink(blogPost.Link);
            if (metadata == null)
            {
                metadata = new BlogMetadata {Link = blogPost.Link};
                blogMetadataStore.Add(metadata);
            }

            blogPost.Metadata = metadata;

            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            foreach (var metadataTag in metadata.Tags)
            {
                var tag = blogTagStore.GetByLink(metadataTag);
                if (tag != null)
                {
                    blogPost.Tags.Add(tag);
                }
            }
        }

        private async Task AggregateStoreAsync(CancellationToken cancellationToken)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);

            foreach (var blogPost in blogPostStore.GetAll())
            {
                await SetPostRawData(blogPost, cancellationToken);
                blogPost.ExtractRuntimeData(_option);
            }
        }
    }
}