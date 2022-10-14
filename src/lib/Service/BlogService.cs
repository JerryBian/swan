using HtmlAgilityPack;
using Laobian.Lib.Cache;
using Laobian.Lib.Extension;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Repository;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Laobian.Lib.Service
{
    public class BlogService : IBlogService
    {
        private const string PostCacheKey = "AllBlogPosts";

        private readonly ICacheManager _cacheManager;
        private readonly IBlogRepository _blogRepository;
        private readonly ILogger<BlogService> _logger;

        public BlogService(ICacheManager cacheManager, IBlogRepository readRepository, ILogger<BlogService> logger)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _blogRepository = readRepository;
        }

        public async Task<List<BlogPostView>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            return await _cacheManager.GetOrCreateAsync(PostCacheKey, async () =>
            {
                var sw = Stopwatch.StartNew();
                List<BlogPostView> result = new();
                await foreach (BlogPost item in _blogRepository.ReadAllPostsAsync(cancellationToken))
                {
                    var htmlDoc = GetPostHtmlDoc(item.MdContent);
                    BlogPostView view = new(item)
                    {
                        ExcerptText = GetPostExcerpt(htmlDoc),
                        HtmlContent = htmlDoc.DocumentNode.OuterHtml,
                        FullLink = $"/blog/{item.PublishTime:yyyy/MM}/{item.Link.ToLowerInvariant()}.html",
                        Metadata = $"<i class=\"bi bi-calendar2-date\"></i> {item.PublishTime.ToCnDate()} &middot; <i class=\"bi bi-eye\"></i> {item.AccessCount} 次阅读",
                    };

                    result.Add(view);
                }

                sw.Stop();
                _logger.LogInformation($"Reloaded all blog posts, elapsed {sw.ElapsedMilliseconds}ms.");
                return result;
            });
        }

        public async Task<BlogPostView> GetPostAsync(string id, CancellationToken cancellationToken = default)
        {
            List<BlogPostView> items = await GetAllPostsAsync(cancellationToken);
            return items.FirstOrDefault(x => x.Raw.Id == id);
        }

        public async Task<BlogPostView> GetPostAsync(int year, int month, string link, CancellationToken cancellationToken = default)
        {
            List<BlogPostView> items = await GetAllPostsAsync(cancellationToken);
            return items.FirstOrDefault(x => x.Raw.PublishTime.Year == year && x.Raw.PublishTime.Month == month && string.Equals(x.Raw.Link, link, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<BlogPostView> AddPostAsync(BlogPost item, CancellationToken cancellationToken = default)
        {
            await _blogRepository.AddPostAsync(item, cancellationToken);
            ClearCache();
            return await GetPostAsync(item.Id, cancellationToken);
        }

        public async Task<BlogPostView> UpdateAsync(BlogPost item, CancellationToken cancellationToken = default)
        {
            await _blogRepository.UpdatePostAsync(item, cancellationToken);
            ClearCache();
            return await GetPostAsync(item.Id, cancellationToken);
        }

        public async Task<bool> AddPostAccessAsync(string id, int count, CancellationToken cancellationToken = default)
        {
            return await _blogRepository.AddPostAccessAsync(id, count, cancellationToken);
        }

        private string GetPostExcerpt(HtmlDocument htmlDoc)
        {
            var excerptText = string.Empty;
            var paraNodes =
                htmlDoc.DocumentNode
                    .Descendants()
                    .Where(_ =>
                        StringHelper.EqualsIgoreCase(_.Name, "p") &&
                        _.Descendants().FirstOrDefault(c => StringHelper.EqualsIgoreCase(c.Name, "img")) == null
                        && _.InnerText.Length > 5)
                    .FirstOrDefault();
            if(paraNodes != null)
            {
                excerptText += paraNodes.InnerText[..Math.Min(120, paraNodes.InnerText.Length)];
            }

            return excerptText;
        }

        private HtmlDocument GetPostHtmlDoc(string mdContent)
        {
            var html = MarkdownHelper.ToHtml(mdContent);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
            foreach (var imageNode in imageNodes)
            {
                if (imageNode.Attributes.Contains("src"))
                {
                    imageNode.AddClass("img-thumbnail mx-auto d-block");
                    imageNode.Attributes.Add("loading", "lazy");
                }
            }

            var tableNodes = htmlDoc.DocumentNode.Descendants("table").ToList();
            foreach (var tableNode in tableNodes)
            {
                tableNode.AddClass("table table-striped table-bordered table-responsive");
            }

            var h3 = htmlDoc.DocumentNode.Descendants("h3").ToList();
            foreach(var h3Node in h3)
            {
                h3Node.Id = h3Node.InnerText;
                h3Node.InnerHtml = $"<i class=\"bi bi-dash small text-dark\"></i> {h3Node.InnerHtml} <i class=\"bi bi-dash small text-dark\"></i>";
            }

            var h4 = htmlDoc.DocumentNode.Descendants("h4").ToList();
            foreach (var h4Node in h4)
            {
                h4Node.Id = h4Node.InnerText;
                h4Node.InnerHtml = $"<i class=\"bi bi-dash small text-secondary\"></i> {h4Node.InnerHtml} <i class=\"bi bi-dash small text-secondary\"></i>";
            }

            return htmlDoc;
        }

        private void ClearCache()
        {
            _cacheManager.TryRemove(PostCacheKey);
        }
    }
}
