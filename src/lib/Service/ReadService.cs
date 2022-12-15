using Swan.Lib.Cache;
using Swan.Lib.Extension;
using Swan.Lib.Helper;
using Swan.Lib.Model;
using Swan.Lib.Repository;

namespace Swan.Lib.Service
{
    public class ReadService : IReadService
    {
        private const string CacheKey = "AllReadItem";

        private readonly ICacheManager _cacheManager;
        private readonly IBlogService _blogService;
        private readonly IReadRepository _readRepository;

        public ReadService(ICacheManager cacheManager, IReadRepository readRepository, IBlogService blogService)
        {
            _cacheManager = cacheManager;
            _readRepository = readRepository;
            _blogService = blogService;
        }

        public async Task<List<ReadItemView>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _cacheManager.GetOrCreateAsync(CacheKey, async () =>
            {
                List<ReadItemView> result = new();
                await foreach (ReadItem item in _readRepository.ReadAllAsync(cancellationToken))
                {
                    ReadItemView view = new(item);
                    List<string> metadata = new();
                    string author = item.Author;
                    if (!string.IsNullOrEmpty(author))
                    {
                        if (!string.IsNullOrEmpty(item.AuthorCountry))
                        {
                            author = $"{author}({item.AuthorCountry})";
                        }

                        metadata.Add(author);
                    }

                    if (!string.IsNullOrEmpty(item.Translator))
                    {
                        metadata.Add($"{item.Translator}(译)");
                    }

                    if (!string.IsNullOrEmpty(item.PublisherName))
                    {
                        metadata.Add(item.PublisherName);
                    }

                    if (item.PublishDate != default)
                    {
                        metadata.Add(item.PublishDate.ToDate());
                    }

                    view.Metadata = string.Join(" / ", metadata);
                    view.CommentHtml = MarkdownHelper.ToHtml(item.Comment);

                    if (item.Posts != null && item.Posts.Any())
                    {
                        foreach (string p in item.Posts)
                        {
                            BlogPostView post = await _blogService.GetPostAsync(p);
                            if (post != null)
                            {
                                view.Posts.Add(new Tuple<string, string, string>(post.Raw.Id, post.Raw.Title, post.Raw.Link));
                            }
                        }
                    }

                    result.Add(view);
                }

                return result.OrderBy(x => x.Raw.CreateTime).ToList();
            });
        }

        public async Task<ReadItemView> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            List<ReadItemView> items = await GetAllAsync(cancellationToken);
            return items.FirstOrDefault(x => x.Raw.Id == id);
        }

        public async Task AddAsync(ReadItem item, CancellationToken cancellationToken = default)
        {
            await _readRepository.AddAsync(item, cancellationToken);
            ClearCache();
        }

        public async Task UpdateAsync(ReadItem item, CancellationToken cancellationToken = default)
        {
            await _readRepository.UpdateAsync(item, cancellationToken);
            ClearCache();
        }

        private void ClearCache()
        {
            _cacheManager.TryRemove(CacheKey);
        }
    }
}
