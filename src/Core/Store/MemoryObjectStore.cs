using HtmlAgilityPack;
using Swan.Core.Cache;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;

namespace Swan.Core.Store
{
    public class MemoryObjectStore : IMemoryObjectStore
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ICacheManager _cacheManager;
        private readonly IFileObjectStore<ReadObject> _readObjectStore;
        private readonly IFileObjectStore<BlogTagObject> _blogTagObjectStore;
        private readonly IFileObjectStore<BlogSeriesObject> _blogSeriesObjectStore;
        private readonly IFileObjectStore<BlogPostObject> _blogPostObjectStore;

        public MemoryObjectStore(
            ICacheManager cacheManager,
            IFileObjectStore<ReadObject> readObjectStore,
            IFileObjectStore<BlogTagObject> blogTagObjectStore,
            IFileObjectStore<BlogSeriesObject> blogSeriesObjectStore,
            IFileObjectStore<BlogPostObject> blogPostObjectStore)
        {
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
            var obj = await GetMemoryObjectAsync(isAdmin);
            return obj.BlogPosts;
        }

        public async Task<BlogPost> AddPostAsync(BlogPostObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _blogPostObjectStore.AddAsync(obj);
                ClearCache();

                var posts = await GetBlogPostsAsync(true);
                return posts.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<BlogPost> UpdatePostAsync(BlogPostObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _blogPostObjectStore.UpdateAsync(obj);
                ClearCache();

                var posts = await GetBlogPostsAsync(true);
                return posts.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        #endregion

        #region Blog Tags

        public async Task<List<BlogTag>> GetBlogTagsAsync(bool isAdmin)
        {
            var obj = await GetMemoryObjectAsync(isAdmin);
            return obj.BlogTags;
        }

        public async Task<BlogTag> AddTagAsync(BlogTagObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _blogTagObjectStore.AddAsync(obj);
                ClearCache();

                var tags = await GetBlogTagsAsync(true);
                return tags.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<BlogTag> UpdateTagAsync(BlogTagObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _blogTagObjectStore.UpdateAsync(obj);
                ClearCache();

                var tags = await GetBlogTagsAsync(true);
                return tags.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task DeleteTagAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var tags = await GetBlogTagsAsync(true);
                var tag = tags.FirstOrDefault(x => x.Object.Id == id);
                if (tag == null)
                {
                    return;
                }

                foreach (var post in tag.Posts)
                {
                    post.Object.Tags.Remove(id);
                    await _blogPostObjectStore.UpdateAsync(post.Object);
                }

                await _blogTagObjectStore.DeleteAsync(id);
                ClearCache();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        #endregion

        #region Blog Series

        public async Task<List<BlogSeries>> GetBlogSeriesAsync(bool isAdmin)
        {
            var obj = await GetMemoryObjectAsync(isAdmin);
            return obj.BlogSeries;
        }

        public async Task<BlogSeries> AddSeriesAsync(BlogSeriesObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _blogSeriesObjectStore.AddAsync(obj);
                ClearCache();

                var series = await GetBlogSeriesAsync(true);
                return series.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<BlogSeries> UpdateSeriesAsync(BlogSeriesObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _blogSeriesObjectStore.UpdateAsync(obj);
                ClearCache();

                var series = await GetBlogSeriesAsync(true);
                return series.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task DeleteSeriesAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var series = await GetBlogSeriesAsync(true);
                var seriesObj = series.FirstOrDefault(x => x.Object.Id == id);
                if (seriesObj == null)
                {
                    return;
                }

                foreach (var post in seriesObj.Posts)
                {
                    post.Object.Tags.Remove(id);
                    await _blogPostObjectStore.UpdateAsync(post.Object);
                }

                await _blogSeriesObjectStore.DeleteAsync(id);
                ClearCache();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        #endregion

        #region Reads

        public async Task<List<ReadModel>> GetReadModelsAsync(bool isAdmin)
        {
            var obj = await GetMemoryObjectAsync(isAdmin);
            return obj.ReadModels;
        }

        public async Task<ReadModel> AddReadAsync(ReadObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _readObjectStore.AddAsync(obj);
                ClearCache();

                var readModel = await GetReadModelsAsync(true);
                return readModel.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<ReadModel> UpdateReadAsync(ReadObject obj)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var result = await _readObjectStore.UpdateAsync(obj);
                ClearCache();

                var readModel = await GetReadModelsAsync(true);
                return readModel.First(x => x.Object.Id == result.Id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task DeleteReadAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var readModels = await GetReadModelsAsync(true);
                var readModel = readModels.FirstOrDefault(x => x.Object.Id == id);
                if (readModel == null)
                {
                    return;
                }

                await _readObjectStore.DeleteAsync(id);
                ClearCache();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        #endregion

        private void ClearCache()
        {
            _cacheManager.TryRemove(Constants.CacheKey.MemoryObjectsAdmin);
        }

        private async Task<MemoryObject> GetMemoryObjectAsync(bool isAdmin)
        {
            var cacheKey = isAdmin ? Constants.CacheKey.MemoryObjectsAdmin : Constants.CacheKey.MemoryObjects;
            return await _cacheManager.GetOrCreateAsync(cacheKey, async () =>
            {
                return await GetMemoryObjectFromStoreAsync(isAdmin);
            });
        }

        private async Task<MemoryObject> GetMemoryObjectFromStoreAsync(bool isAdmin)
        {
            var blogPostObjs = await _blogPostObjectStore.GetAllAsync();
            var blogTagObjs = await _blogTagObjectStore.GetAllAsync();
            var blogSeriesObjs = await _blogSeriesObjectStore.GetAllAsync();
            var readModelObjs = await _readObjectStore.GetAllAsync();

            var blogPosts = new List<BlogPost>();
            foreach (var obj in blogPostObjs)
            {
                var post = new BlogPost(obj);
                if(isAdmin || post.IsPublished())
                {
                    var htmlDoc = GetPostHtmlDoc(obj.MdContent);
                    post.HtmlContent = htmlDoc.DocumentNode.OuterHtml;
                    blogPosts.Add(post);
                }
            }

            var blogTags = new List<BlogTag>();
            foreach (var obj in blogTagObjs)
            {
                var tag = new BlogTag(obj);
                tag.Posts.AddRange(blogPosts.Where(x => x.Object.Tags.Contains(obj.Id)));
                blogTags.Add(tag);
            }

            var blogSereis = new List<BlogSeries>();
            foreach (var obj in blogSeriesObjs)
            {
                var series = new BlogSeries(obj);
                series.Posts.AddRange(blogPosts.Where(x => x.Object.Series == obj.Id));
                blogSereis.Add(series);
            }

            var readModels = new List<ReadModel>();
            foreach (var obj in readModelObjs)
            {
                if(isAdmin || obj.IsPublic)
                {
                    var readModel = new ReadModel(obj);
                    readModel.Metadata = GetReadMetadata(obj);
                    readModel.CommentHtml = MarkdownHelper.ToHtml(obj.Comment);
                    readModel.BlogPosts.AddRange(blogPosts.Where(x => obj.Posts.Contains(x.Object.Id)));
                    readModels.Add(readModel);
                } 
            }

            foreach (var post in blogPosts)
            {
                post.BlogTags.AddRange(blogTags.Where(x => x.Posts.Contains(post)));
                post.BlogSeries = blogSereis.FirstOrDefault(x => x.Posts.Contains(post));
            }

            if(isAdmin)
            {
                blogTags.RemoveAll(x => !x.Posts.Any());
                blogSereis.RemoveAll(x => !x.Posts.Any());
            }

            var memoryObj = new MemoryObject();
            memoryObj.BlogPosts.AddRange(blogPosts);
            memoryObj.BlogSeries.AddRange(blogSereis);
            memoryObj.BlogTags.AddRange(blogTags);
            memoryObj.ReadModels.AddRange(readModels);
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
                h3Node.AddClass("mb-3 mt-4 border border-secondary px-1 py-2 text-truncate");
                h3Node.InnerHtml = $"<i class=\"bi bi-collection small text-muted pe-1\"></i> {h3Node.InnerHtml}";
            }

            List<HtmlNode> h4 = htmlDoc.DocumentNode.Descendants("h4").ToList();
            foreach (HtmlNode h4Node in h4)
            {
                h4Node.Id = h4Node.InnerText;
                h4Node.InnerHtml = $"<i class=\"bi bi-collection small text-muted pe-1\"></i> {h4Node.InnerHtml} <i class=\"bi bi-dash small text-secondary\"></i>";
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
