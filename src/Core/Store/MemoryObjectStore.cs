using HtmlAgilityPack;
using Swan.Core.Cache;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Swan.Core.Store
{
    public class MemoryObjectStore : IMemoryObjectStore
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ICacheClient _cacheManager;
        private readonly ILogger<MemoryObjectStore> _logger;
        private readonly IFileObjectStore<ReadObject> _readObjectStore;
        private readonly IFileObjectStore<BlogTagObject> _blogTagObjectStore;
        private readonly IFileObjectStore<BlogSeriesObject> _blogSeriesObjectStore;
        private readonly IFileObjectStore<BlogPostObject> _blogPostObjectStore;

        public MemoryObjectStore(
            ICacheClient cacheManager,
            ILogger<MemoryObjectStore> logger,
            IFileObjectStore<ReadObject> readObjectStore,
            IFileObjectStore<BlogTagObject> blogTagObjectStore,
            IFileObjectStore<BlogSeriesObject> blogSeriesObjectStore,
            IFileObjectStore<BlogPostObject> blogPostObjectStore)
        {
            _logger = logger;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _cacheManager = cacheManager;
            _readObjectStore = readObjectStore;
            _blogPostObjectStore = blogPostObjectStore;
            _blogTagObjectStore = blogTagObjectStore;
            _blogSeriesObjectStore = blogSeriesObjectStore;
        }

        #region Blog Posts

        public async Task<List<BlogPost>> GetBlogPostsAsync(bool isAdmin)
        {
            MemoryObject obj = await GetMemoryObjectAsync(isAdmin);
            return obj.BlogPosts;
        }

        public async Task<BlogPost> AddPostAsync(BlogPostObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogPostObject result = await _blogPostObjectStore.AddAsync(obj);

                List<BlogPost> posts = await GetBlogPostsAsync(true);
                return posts.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task<BlogPost> UpdatePostAsync(BlogPostObject obj, bool coreUpdate)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogPostObject result = await _blogPostObjectStore.UpdateAsync(obj, coreUpdate);

                List<BlogPost> posts = await GetBlogPostsAsync(true);
                return posts.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        #endregion

        #region Blog Tags

        public async Task<List<BlogTag>> GetBlogTagsAsync(bool isAdmin)
        {
            MemoryObject obj = await GetMemoryObjectAsync(isAdmin);
            return obj.BlogTags;
        }

        public async Task<BlogTag> AddTagAsync(BlogTagObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogTagObject result = await _blogTagObjectStore.AddAsync(obj);

                List<BlogTag> tags = await GetBlogTagsAsync(true);
                return tags.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task<BlogTag> UpdateTagAsync(BlogTagObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogTagObject result = await _blogTagObjectStore.UpdateAsync(obj);

                List<BlogTag> tags = await GetBlogTagsAsync(true);
                return tags.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task DeleteTagAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                List<BlogTag> tags = await GetBlogTagsAsync(true);
                BlogTag tag = tags.FirstOrDefault(x => x.Object.Id == id);
                if (tag == null)
                {
                    return;
                }

                foreach (BlogPost post in tag.Posts)
                {
                    _ = post.Object.Tags.Remove(id);
                    _ = await _blogPostObjectStore.UpdateAsync(post.Object);
                }

                await _blogTagObjectStore.DeleteAsync(x => x.Id == id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        #endregion

        #region Blog Series

        public async Task<List<BlogSeries>> GetBlogSeriesAsync(bool isAdmin)
        {
            MemoryObject obj = await GetMemoryObjectAsync(isAdmin);
            return obj.BlogSeries;
        }

        public async Task<BlogSeries> AddSeriesAsync(BlogSeriesObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogSeriesObject result = await _blogSeriesObjectStore.AddAsync(obj);

                List<BlogSeries> series = await GetBlogSeriesAsync(true);
                return series.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task<BlogSeries> UpdateSeriesAsync(BlogSeriesObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogSeriesObject result = await _blogSeriesObjectStore.UpdateAsync(obj);

                List<BlogSeries> series = await GetBlogSeriesAsync(true);
                return series.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task DeleteSeriesAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                List<BlogSeries> series = await GetBlogSeriesAsync(true);
                BlogSeries seriesObj = series.FirstOrDefault(x => x.Object.Id == id);
                if (seriesObj == null)
                {
                    return;
                }

                foreach (BlogPost post in seriesObj.Posts)
                {
                    _ = post.Object.Tags.Remove(id);
                    _ = await _blogPostObjectStore.UpdateAsync(post.Object);
                }

                await _blogSeriesObjectStore.DeleteAsync(x => x.Id == id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        #endregion

        #region Reads

        public async Task<List<ReadModel>> GetReadModelsAsync(bool isAdmin)
        {
            MemoryObject obj = await GetMemoryObjectAsync(isAdmin);
            return obj.ReadModels;
        }

        public async Task<ReadModel> AddReadAsync(ReadObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                ReadObject result = await _readObjectStore.AddAsync(obj);

                List<ReadModel> readModel = await GetReadModelsAsync(true);
                return readModel.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task<ReadModel> UpdateReadAsync(ReadObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                ReadObject result = await _readObjectStore.UpdateAsync(obj);

                List<ReadModel> readModel = await GetReadModelsAsync(true);
                return readModel.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task DeleteReadAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                List<ReadModel> readModels = await GetReadModelsAsync(true);
                ReadModel readModel = readModels.FirstOrDefault(x => x.Object.Id == id);
                if (readModel == null)
                {
                    return;
                }

                await _readObjectStore.DeleteAsync(x => x.Id == id);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        #endregion

        private async Task<MemoryObject> GetMemoryObjectAsync(bool isAdmin)
        {
            return isAdmin
                ? await GetMemoryObjectFromStoreAsync(true)
                : await _cacheManager.GetOrCreateAsync(Constants.CacheKey.MemoryObjects, async () =>
            {
                return await GetMemoryObjectFromStoreAsync(false);
            }, TimeSpan.FromHours(2));
        }

        private async Task<MemoryObject> GetMemoryObjectFromStoreAsync(bool isAdmin)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Task<IEnumerable<BlogPostObject>> blogPostObjsTask = _blogPostObjectStore.GetAllAsync();
            Task<IEnumerable<BlogTagObject>> blogTagObjsTask = _blogTagObjectStore.GetAllAsync();
            Task<IEnumerable<BlogSeriesObject>> blogSeriesObjsTask = _blogSeriesObjectStore.GetAllAsync();
            Task<IEnumerable<ReadObject>> readModelObjsTask = _readObjectStore.GetAllAsync();

            await Task.WhenAll(blogPostObjsTask, blogTagObjsTask, blogSeriesObjsTask, readModelObjsTask);

            ConcurrentBag<BlogPost> blogPosts = new();
            _ = Parallel.ForEach(blogPostObjsTask.Result, obj =>
            {
                BlogPost post = new(obj);
                if (isAdmin || post.IsPublished())
                {
                    HtmlDocument htmlDoc = GetPostHtmlDoc(obj.MdContent);
                    post.HtmlContent = htmlDoc.DocumentNode.OuterHtml;
                    blogPosts.Add(post);
                }
            });


            List<BlogTag> blogTags = new();
            foreach (BlogTagObject obj in blogTagObjsTask.Result)
            {
                BlogTag tag = new(obj);
                tag.Posts.AddRange(blogPosts.Where(x => x.Object.Tags.Contains(obj.Id)));
                blogTags.Add(tag);
            }

            List<BlogSeries> blogSereis = new();
            foreach (BlogSeriesObject obj in blogSeriesObjsTask.Result)
            {
                BlogSeries series = new(obj);
                series.Posts.AddRange(blogPosts.Where(x => x.Object.Series == obj.Id));
                blogSereis.Add(series);
            }

            ConcurrentBag<ReadModel> readModels = new();
            _ = Parallel.ForEach(readModelObjsTask.Result, obj =>
            {
                if (isAdmin || obj.IsPublic)
                {
                    ReadModel readModel = new(obj)
                    {
                        Metadata = GetReadMetadata(obj),
                        CommentHtml = MarkdownHelper.ToHtml(obj.Comment)
                    };
                    readModel.BlogPosts.AddRange(blogPosts.Where(x => obj.Posts.Contains(x.Object.Id)));
                    readModels.Add(readModel);
                }
            });

            _ = Parallel.ForEach(blogPosts, post =>
            {
                post.BlogTags.AddRange(blogTags.Where(x => x.Posts.Contains(post)));
                post.BlogSeries = blogSereis.FirstOrDefault(x => x.Posts.Contains(post));
            });

            if (!isAdmin)
            {
                _ = blogTags.RemoveAll(x => !x.Posts.Any());
                _ = blogSereis.RemoveAll(x => !x.Posts.Any());
            }

            MemoryObject memoryObj = new();
            Parallel.Invoke(
                () => memoryObj.BlogPosts.AddRange(blogPosts),
                () => memoryObj.BlogSeries.AddRange(blogSereis),
                () => memoryObj.BlogTags.AddRange(blogTags),
                () => memoryObj.ReadModels.AddRange(readModels));

            stopwatch.Stop();
            _logger.LogDebug($"Load memory object in {stopwatch.ElapsedMilliseconds}ms, isAdmin: {isAdmin}");
            return memoryObj;
        }

        private string GetReadMetadata(ReadObject obj)
        {
            List<string> metadata = new();
            string author = obj.Author;
            if (!string.IsNullOrEmpty(author))
            {
                if (!string.IsNullOrEmpty(obj.AuthorCountry))
                {
                    author = $"{author}({obj.AuthorCountry})";
                }

                metadata.Add(author);
            }

            if (!string.IsNullOrEmpty(obj.Translator))
            {
                metadata.Add($"{obj.Translator}(译)");
            }

            if (!string.IsNullOrEmpty(obj.PublisherName))
            {
                metadata.Add(obj.PublisherName);
            }

            if (obj.PublishDate != default)
            {
                metadata.Add(obj.PublishDate.ToDate());
            }

            return string.Join(" / ", metadata);
        }

        private HtmlDocument GetPostHtmlDoc(string mdContent)
        {
            string html = MarkdownHelper.ToHtml(mdContent);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            List<HtmlNode> imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
            foreach (HtmlNode imageNode in imageNodes)
            {
                if (imageNode.Attributes.Contains("src"))
                {
                    imageNode.AddClass("img-thumbnail mx-auto d-block");
                    imageNode.Attributes.Add("loading", "lazy");
                }
            }

            List<HtmlNode> tableNodes = htmlDoc.DocumentNode.Descendants("table").ToList();
            foreach (HtmlNode tableNode in tableNodes)
            {
                tableNode.AddClass("table table-striped table-bordered table-responsive");
            }

            List<HtmlNode> h3 = htmlDoc.DocumentNode.Descendants("h3").ToList();
            foreach (HtmlNode h3Node in h3)
            {
                h3Node.Id = h3Node.InnerText;
                h3Node.RemoveClass();
                h3Node.AddClass("mb-3 mt-4 border-bottom border-2 border-info px-1 py-2");
                h3Node.Attributes.Add("style", "--bs-border-opacity: .1;");
                h3Node.InnerHtml = $"<i class=\"bi bi-slash small text-muted pe-1\"></i> {h3Node.InnerHtml}";
            }

            List<HtmlNode> h4 = htmlDoc.DocumentNode.Descendants("h4").ToList();
            foreach (HtmlNode h4Node in h4)
            {
                h4Node.Id = h4Node.InnerText;
                h4Node.InnerHtml = $"<span class=\"d-inline-block bg-success p-1 me-2 align-middle\"></span> {h4Node.InnerHtml}";
            }

            return htmlDoc;
        }

        private class MemoryObject
        {
            public List<BlogPost> BlogPosts { get; init; } = new();

            public List<BlogTag> BlogTags { get; init; } = new();

            public List<BlogSeries> BlogSeries { get; init; } = new();

            public List<ReadModel> ReadModels { get; init; } = new();
        }
    }
}
