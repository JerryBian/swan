using HtmlAgilityPack;
using Swan.Core.Cache;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class BlogService : IBlogService
    {
        private const string AllPostsKey = "All_Posts";
        private const string AllTagsKey = "All_Tags";
        private const string AllSeriesKey = "All_Series";

        private readonly IFileObjectStore<BlogTagObject> _blogTagObjectStore;
        private readonly IFileObjectStore<BlogSeriesObject> _blogSeriesObjectStore;
        private readonly IFileObjectStore<BlogPostObject> _blogPostObjectStore;
        private readonly ICacheManager _cacheManager;

        public BlogService(
            ICacheManager cacheManager,
            IFileObjectStore<BlogTagObject> blogTagObjectStore,
            IFileObjectStore<BlogSeriesObject> blogSeriesObjectStore,
            IFileObjectStore<BlogPostObject> blogPostObjectStore)
        {
            _cacheManager = cacheManager;
            _blogPostObjectStore = blogPostObjectStore;
            _blogTagObjectStore = blogTagObjectStore;
            _blogSeriesObjectStore = blogSeriesObjectStore;
        }

        #region Posts

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            return await _cacheManager.GetOrCreateAsync(AllPostsKey, async () =>
            {
                var result = new List<BlogPost>();
                var objs = await _blogPostObjectStore.GetAllAsync();
                //var allSeries = await GetAllSeriesAsync();
                //var allTags = await GetAllTagsAsync();
                foreach (var obj in objs)
                {
                    var post = new BlogPost(obj);
                    var htmlDoc = GetPostHtmlDoc(obj.MdContent);
                    post.HtmlContent = htmlDoc.DocumentNode.OuterHtml;
                    result.Add(post);
                }

                return result.OrderByDescending(x => x.Object.PublishTime).ToList();
            });
        }

        public async Task<BlogPost> GetPostAsync(string id)
        {
            var allPosts = await GetAllPostsAsync();
            return allPosts.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<BlogPost> GetPostByLinkAsync(string link)
        {
            var allPosts = await GetAllPostsAsync();
            return allPosts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(link, x.Object.Link));
        }

        public async Task<BlogPostObject> CreatePostAsync(BlogPostObject obj)
        {
            var result = await _blogPostObjectStore.AddAsync(obj);
            ClearCache();
            return result;
        }

        public async Task<BlogPostObject> UpdatePostAsync(BlogPostObject obj)
        {
            var result = await _blogPostObjectStore.UpdateAsync(obj);
            ClearCache();
            return result;
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

        #endregion

        #region Tags

        public async Task<BlogTag> GetTagAsync(string id)
        {
            var tags = await GetAllTagsAsync();
            return tags.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<BlogTag> GetTagByUrlAsync(string url)
        {
            var tags = await GetAllTagsAsync();
            return tags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Object.Url, url));
        }

        public async Task<List<BlogTag>> GetAllTagsAsync()
        {
            return await _cacheManager.GetOrCreateAsync(AllTagsKey, async () =>
            {
                var result = new List<BlogTag>();
                var objs = await _blogTagObjectStore.GetAllAsync();
                var allPosts = await GetAllPostsAsync();
                foreach (var obj in objs)
                {
                    var tag = new BlogTag(obj);
                    tag.Posts.AddRange(allPosts.Where(x => x.Object.Tags.Contains(obj.Id)));
                    result.Add(tag);
                }

                return result;
            });
        }

        public async Task<BlogTagObject> CreateTagAsync(BlogTagObject obj)
        {
            var result = await _blogTagObjectStore.AddAsync(obj);
            ClearCache();
            return result;
        }

        public async Task<BlogTagObject> UpdateTagAsync(BlogTagObject obj)
        {
            var result = await _blogTagObjectStore.UpdateAsync(obj);
            ClearCache();
            return result;
        }

        public async Task DeleteTagAsync(string id)
        {
            await _blogTagObjectStore.DeleteAsync(id);
            ClearCache();
        }

        #endregion

        #region Series

        public async Task<BlogSeries> GetSeriesAsync(string id)
        {
            var allSeries = await GetAllSeriesAsync();
            return allSeries.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<BlogSeries> GetSeriesByUrlAsync(string url)
        {
            var allSeries = await GetAllSeriesAsync();
            return allSeries.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Object.Url, url));
        }

        public async Task<List<BlogSeries>> GetAllSeriesAsync()
        {
            return await _cacheManager.GetOrCreateAsync(AllSeriesKey, async () =>
            {
                var result = new List<BlogSeries>();
                var objs = await _blogSeriesObjectStore.GetAllAsync();
                foreach (var obj in objs)
                {
                    var series = new BlogSeries(obj);
                    result.Add(series);
                }

                return result;
            });
        }

        public async Task<BlogSeriesObject> CreateSeriesAsync(BlogSeriesObject obj)
        {
            var result = await _blogSeriesObjectStore.AddAsync(obj);
            ClearCache();
            return result;
        }

        public async Task<BlogSeriesObject> UpdateSeriesAsync(BlogSeriesObject obj)
        {
            var result = await _blogSeriesObjectStore.UpdateAsync(obj);
            ClearCache();
            return result;
        }

        public async Task DeleteSeriesAsync(string id)
        {
            await _blogSeriesObjectStore.DeleteAsync(id);
            ClearCache();
        }

        #endregion

        private void ClearCache()
        {
            _cacheManager.TryRemove(AllTagsKey);
            _cacheManager.TryRemove(AllSeriesKey);
            _cacheManager.TryRemove(AllPostsKey);
        }
    }
}
