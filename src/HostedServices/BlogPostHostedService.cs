using Swan.Core.Extension;
using Swan.Core.Model.Object;
using Swan.Core.Service;
using Swan.Core.Store;

namespace Swan.HostedServices
{
    public class BlogPostHostedService : BackgroundService
    {
        private readonly IBlogPostAccessStore _blogPostAccessStore;
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogPostHostedService> _logger;
        private readonly IFileObjectStore<BlogPostAccessObject> _store;

        public BlogPostHostedService(
            ILogger<BlogPostHostedService> logger,
            IFileObjectStore<BlogPostAccessObject> store,
            IBlogService blogService,
            IBlogPostAccessStore blogPostAccessStore)
        {
            _logger = logger;
            _store = store;
            _blogService = blogService;
            _blogPostAccessStore = blogPostAccessStore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken).OkForCancel();

                    List<BlogPostAccessObject> items = _blogPostAccessStore.DequeueAll();
                    if (!items.Any())
                    {
                        continue;
                    }

                    await _store.DeleteAsync(x => DateTime.Now - x.CreateTime > TimeSpan.FromDays(15));

                    IEnumerable<BlogPostAccessObject> objs = await _store.GetAllAsync();
                    Dictionary<string, List<BlogPostAccessObject>> bufferedItems = new();
                    foreach (IGrouping<string, BlogPostAccessObject> item in items.GroupBy(x => x.PostId))
                    {
                        Core.Model.BlogPost post = await _blogService.GetPostAsync(item.Key);
                        if (post == null)
                        {
                            continue;
                        }

                        var hasStoredItem = false;
                        var candidateItems = new List<BlogPostAccessObject>(item);
                        var latestStoredItem = objs.Where(x => x.Id == item.Key).OrderByDescending(x => x.Timestamp).FirstOrDefault();
                        if (latestStoredItem != null)
                        {
                            hasStoredItem = true;
                            candidateItems.Add(latestStoredItem);
                        }

                        var validItems = new List<BlogPostAccessObject>();
                        foreach (var item1 in candidateItems.OrderBy(x => x.Timestamp))
                        {
                            var lastValidItem = validItems.LastOrDefault();
                            if(lastValidItem == null || item1.Timestamp - lastValidItem.Timestamp > TimeSpan.FromMinutes(1))
                            {
                                validItems.Add(item1);
                            }
                        }

                        post.Object.AccessCount += hasStoredItem ? validItems.Count - 1 : validItems.Count;
                        _ = await _blogService.UpdatePostAsync(post.Object, false);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Blog post hosted service exection has error.");
                }
            }
        }
    }
}