using DotNext.Threading;
using GitStoreDotnet;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    internal class SwanService : ISwanService
    {
        private readonly IGitStore _gitStore;
        private readonly ISwanStore _swanStore;
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;

        public SwanService(IGitStore gitStore, ISwanStore swanStore)
        {
            _gitStore = gitStore;
            _swanStore = swanStore;
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
        }

        public async Task<List<BlogPost>> GetBlogPostsAsync()
        {
            await _asyncReaderWriterLock.EnterReadLockAsync();

            try
            {
                var obj = await _swanStore.GetAsync();
                return obj.BlogPosts;
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task AddBlogPostAsync(BlogPost blogPost)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                var obj = await _swanStore.GetAsync();
                if(obj.BlogPosts.Find(x => StringHelper.EqualsIgoreCase(x.Id, blogPost.Id)) != null)
                {
                    throw new Exception($"Blog post with id {blogPost.Id} already exists.");
                }

                if (obj.BlogPosts.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogPost.Link)) != null)
                {
                    throw new Exception($"Blog post with link {blogPost.Link} already exists.");
                }

                blogPost.CreatedAt = blogPost.LastUpdatedAt = DateTime.Now;
                var posts = new List<BlogPost>(obj.BlogPosts) { blogPost };
                var content = JsonHelper.Serialize(posts.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogPost.GitStorePath, content, true);

                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task UpdateBlogPostAsync(BlogPost blogPost)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                var obj = await _swanStore.GetAsync();
                var oldPost = obj.BlogPosts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, blogPost.Id));
                if (oldPost == null)
                {
                    throw new Exception($"Blog post with id {blogPost.Id} not exists.");
                }

                var allPosts = new List<BlogPost>(obj.BlogPosts);
                allPosts.Remove(oldPost);

                if (allPosts.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogPost.Link)) != null)
                {
                    throw new Exception($"Blog post with link {blogPost.Link} already exists.");
                }

                blogPost.CreatedAt = oldPost.CreatedAt;
                blogPost.LastUpdatedAt = DateTime.Now;
                allPosts.Add(blogPost);
                var content = JsonHelper.Serialize(allPosts.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogPost.GitStorePath, content, true);

                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task DeleteBlogPostAsync(string id)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task<List<BlogTag>> GetBlogTagsAsync()
        {
            await _asyncReaderWriterLock.EnterReadLockAsync();

            try
            {
                StoreObject obj = await _swanStore.GetAsync();
                return obj.BlogTags;
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task AddBlogTagAsync(BlogTag blogTag)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task UpdateBlogTagAsync(BlogTag blogTag)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task DeleteBlogTagAsync(string id)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task<List<BlogSeries>> GetBlogSeriesAsync()
        {
            await _asyncReaderWriterLock.EnterReadLockAsync();

            try
            {
                StoreObject obj = await _swanStore.GetAsync();
                return obj.BlogSeries;
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task AddBlogSeriesAsync(BlogSeries blogSeries)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task UpdateBlogSeriesAsync(BlogSeries blogSeries)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task DeleteBlogSeriesAsync(string id)
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }
    }
}
