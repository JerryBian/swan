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
                StoreObject obj = await _swanStore.GetAsync();
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
                StoreObject obj = await _swanStore.GetAsync();
                if (obj.BlogPosts.Find(x => StringHelper.EqualsIgoreCase(x.Id, blogPost.Id)) != null)
                {
                    throw new Exception($"Blog post with id {blogPost.Id} already exists.");
                }

                if (obj.BlogPosts.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogPost.Link)) != null)
                {
                    throw new Exception($"Blog post with link {blogPost.Link} already exists.");
                }

                blogPost.CreatedAt = blogPost.LastUpdatedAt = DateTime.Now;
                List<BlogPost> posts = new List<BlogPost>(obj.BlogPosts) { blogPost };
                string content = JsonHelper.Serialize(posts.OrderByDescending(x => x.CreatedAt));
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
                StoreObject obj = await _swanStore.GetAsync();
                BlogPost oldPost = obj.BlogPosts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, blogPost.Id));
                if (oldPost == null)
                {
                    throw new Exception($"Blog post with id {blogPost.Id} not exists.");
                }

                List<BlogPost> allPosts = new List<BlogPost>(obj.BlogPosts);
                allPosts.Remove(oldPost);

                if (allPosts.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogPost.Link)) != null)
                {
                    throw new Exception($"Blog post with link {blogPost.Link} already exists.");
                }

                blogPost.CreatedAt = oldPost.CreatedAt;
                blogPost.LastUpdatedAt = DateTime.Now;
                allPosts.Add(blogPost);
                string content = JsonHelper.Serialize(allPosts.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogPost.GitStorePath, content, true);

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
                StoreObject obj = await _swanStore.GetAsync();
                if (obj.BlogTags.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogTag.Link)) != null)
                {
                    throw new Exception($"Blog tag with link {blogTag.Link} already exists.");
                }

                blogTag.Id = StringHelper.Random();
                blogTag.CreatedAt = blogTag.LastUpdatedAt = DateTime.Now;
                List<BlogTag> tags = new List<BlogTag>(obj.BlogTags) { blogTag };
                string content = JsonHelper.Serialize(tags.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogTag.GitStorePath, content, true);

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
                StoreObject obj = await _swanStore.GetAsync();
                BlogTag oldTag = obj.BlogTags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, blogTag.Id));
                if (oldTag == null)
                {
                    throw new Exception($"Blog tag with id {blogTag.Id} not exists.");
                }

                List<BlogTag> tags = new List<BlogTag>(obj.BlogTags);
                tags.Remove(oldTag);
                if (tags.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogTag.Link)) != null)
                {
                    throw new Exception($"Blog tag with link {blogTag.Link} already exists.");
                }

                blogTag.CreatedAt = oldTag.CreatedAt;
                blogTag.LastUpdatedAt = DateTime.Now;
                tags.Add(blogTag);
                string content = JsonHelper.Serialize(tags.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogTag.GitStorePath, content, true);

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
                StoreObject obj = await _swanStore.GetAsync();
                BlogTag tag = obj.BlogTags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, id));
                if (tag == null)
                {
                    return;
                }

                List<BlogTag> tags = new List<BlogTag>(obj.BlogTags);
                tags.Remove(tag);
                string content = JsonHelper.Serialize(tags.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogTag.GitStorePath, content, true);

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
                StoreObject obj = await _swanStore.GetAsync();
                if (obj.BlogSeries.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogSeries.Link)) != null)
                {
                    throw new Exception($"Blog tag with link {blogSeries.Link} already exists.");
                }

                blogSeries.Id = StringHelper.Random();
                blogSeries.CreatedAt = blogSeries.LastUpdatedAt = DateTime.Now;
                List<BlogSeries> tags = new List<BlogSeries>(obj.BlogSeries) { blogSeries };
                string content = JsonHelper.Serialize(tags.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogSeries.GitStorePath, content, true);

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
                StoreObject obj = await _swanStore.GetAsync();
                BlogSeries oldSeries = obj.BlogSeries.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, blogSeries.Id));
                if (oldSeries == null)
                {
                    throw new Exception($"Blog series with id {blogSeries.Id} not exists.");
                }

                List<BlogSeries> allSeries = new List<BlogSeries>(obj.BlogSeries);
                allSeries.Remove(oldSeries);
                if (allSeries.Find(x => StringHelper.EqualsIgoreCase(x.Link, blogSeries.Link)) != null)
                {
                    throw new Exception($"Blog series with link {blogSeries.Link} already exists.");
                }

                blogSeries.CreatedAt = oldSeries.CreatedAt;
                blogSeries.LastUpdatedAt = DateTime.Now;
                allSeries.Add(blogSeries);
                string content = JsonHelper.Serialize(allSeries.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogSeries.GitStorePath, content, true);

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
                StoreObject obj = await _swanStore.GetAsync();
                BlogSeries series = obj.BlogSeries.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, id));
                if (series == null)
                {
                    return;
                }

                List<BlogSeries> allSeries = new List<BlogSeries>(obj.BlogSeries);
                allSeries.Remove(series);
                string content = JsonHelper.Serialize(allSeries.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(BlogSeries.GitStorePath, content, true);

                _swanStore.Clear();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }
    }
}
