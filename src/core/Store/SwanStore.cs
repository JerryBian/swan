using GitStoreDotnet;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Swan.Core.Helper;
using Swan.Core.Model;

namespace Swan.Core.Store
{
    internal class SwanStore : ISwanStore
    {
        private const string CacheKey = "core.swanstore";

        private readonly IGitStore _gitStore;
        private readonly ILogger<SwanStore> _logger;
        private readonly IMemoryCache _memoryCache;

        public SwanStore(IGitStore gitStore, ILogger<SwanStore> logger, IMemoryCache memoryCahce)
        {
            _logger = logger;
            _gitStore = gitStore;
            _memoryCache = memoryCahce;
        }

        public async Task<StoreObject> GetAsync()
        {
            return await _memoryCache.GetOrCreateAsync(CacheKey, async entry =>
            {
                StoreObject obj = new StoreObject();
                try
                {
                    obj.BlogPosts = JsonHelper.Deserialize<List<BlogPost>>(await _gitStore.GetTextAsync(BlogPost.GitStorePath, true));
                    obj.BlogSeries = JsonHelper.Deserialize<List<BlogSeries>>(await _gitStore.GetTextAsync(BlogSeries.GitStorePath, true));
                    obj.BlogTags = JsonHelper.Deserialize<List<BlogTag>>(await _gitStore.GetTextAsync(BlogTag.GitStorePath, true));
                    obj.ReadItems = JsonHelper.Deserialize<List<ReadItem>>(await _gitStore.GetTextAsync(ReadItem.GitStorePath, true));

                    PostExtend(obj);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Load swan objects from GitStore failed.");
                }

                return obj;
            });
        }

        public void Clear()
        {
            _memoryCache.Remove(CacheKey);
        }

        private void PostExtend(StoreObject obj)
        {
            obj.BlogPosts ??= new List<BlogPost>();
            obj.BlogSeries ??= new List<BlogSeries>();
            obj.BlogTags ??= new List<BlogTag>();
            obj.ReadItems ??= new List<ReadItem>();

            foreach(var post in obj.BlogPosts)
            {
                var postHtml = MarkdownHelper.ToHtml(post.Content);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(postHtml);
                List<HtmlNode> imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
                foreach (HtmlNode imageNode in imageNodes)
                {
                    if (imageNode.Attributes.Contains("src"))
                    {
                        imageNode.AddClass("img-thumbnail mx-auto d-block");
                        imageNode.Attributes.Add("loading", "lazy");
                    }
                }

                if(string.IsNullOrEmpty(post.Excerpt))
                {
                    var p = htmlDoc.DocumentNode.Descendants("p").FirstOrDefault(x => !string.IsNullOrEmpty(x.InnerText));
                    if(p != null)
                    {
                        post.HtmlExcerpt = MarkdownHelper.ToHtml(p.InnerText);
                    }
                }
                else
                {
                    post.HtmlExcerpt = MarkdownHelper.ToHtml(post.Excerpt);
                }

                post.HtmlContent = htmlDoc.DocumentNode.OuterHtml;

                foreach(var item in post.Tags)
                {
                    var tag = obj.BlogTags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, item));
                    if(tag != null)
                    {
                        post.BlogTags.Add(tag);
                        tag.BlogPosts.Add(post);
                    }
                }

                if(post.Series != null)
                {
                    var series = obj.BlogSeries.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, post.Series));
                    if(series != null)
                    {
                        post.BlogSeries = series;
                        series.BlogPosts.Add(post);
                    }
                }

                post.PreviousPost = obj.BlogPosts.OrderBy(x => x.PublishDate).LastOrDefault(x => x.PublishDate < post.PublishDate);
                post.NextPost = obj.BlogPosts.OrderBy(x => x.PublishDate).FirstOrDefault(x => x.PublishDate > post.PublishDate);
            }

            // random posts recommend
            // TODO: use .NET 8 Random functions
            foreach(var post in obj.BlogPosts)
            {
                if(post.Tags.Any())
                {
                    var similarPosts = new List<BlogPost>();
                    foreach(var item in post.BlogTags)
                    {
                        similarPosts.AddRange(item.BlogPosts);
                    }

                    post.RecommendPostsByTag.AddRange(similarPosts.Distinct().Take(8));
                }

                if(post.Series != null)
                {
                    post.RecommendPostsBySeries.AddRange(post.BlogSeries.BlogPosts.Take(8));
                }
            }
        }
    }
}
