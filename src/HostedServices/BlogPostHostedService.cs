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
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken).OkForCancel();

                List<BlogPostAccessObject> items = _blogPostAccessStore.DequeueAll();
                if (!items.Any())
                {
                    continue;
                }

                IEnumerable<BlogPostAccessObject> objs = await _store.GetAllAsync();
                Dictionary<string, List<BlogPostAccessObject>> bufferedItems = new();
                foreach (IGrouping<string, BlogPostAccessObject> item in items.GroupBy(x => x.PostId))
                {
                    Core.Model.BlogPost post = await _blogService.GetPostAsync(item.Key);
                    if (post == null)
                    {
                        continue;
                    }

                    List<BlogPostAccessObject> removedItems = new();
                    IEnumerable<BlogPostAccessObject> allItems = item.Concat(objs.Where(x => x.Id == item.Key));
                    foreach (BlogPostAccessObject item1 in item)
                    {
                        if (removedItems.Contains(item1))
                        {
                            continue;
                        }

                        if (allItems.Any(x => x.CreateTime - item1.CreateTime < TimeSpan.FromMinutes(1)))
                        {
                            removedItems.Add(item1);
                        }
                    }

                    post.Object.AccessCount += items.Except(removedItems).Count();
                    _ = await _blogService.UpdatePostAsync(post.Object, false);
                }

                await _store.DeleteAsync(x => DateTime.Now - x.CreateTime > TimeSpan.FromDays(15));
            }
        }
    }
}