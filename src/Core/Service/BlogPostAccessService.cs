using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class BlogPostAccessService : IBlogPostAccessService
    {
        private readonly IBlogService _blogService;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IFileObjectStore<BlogPostAccessObject> _store;

        public BlogPostAccessService(IFileObjectStore<BlogPostAccessObject> store, IBlogService blogService)
        {
            _store = store;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _blogService = blogService;
        }

        public async Task AddAsync(string postId, string ipAddress)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                BlogPostAccessObject item = new()
                {
                    PostId = postId,
                    IpAddress = ipAddress,
                    Timestamp = DateTime.Now
                };
                IEnumerable<BlogPostAccessObject> objs = await _store.GetAllAsync();
                BlogPostAccessObject latestObj = objs.Where(x => x.IpAddress == item.IpAddress).OrderByDescending(x => x.Timestamp).FirstOrDefault();
                if (latestObj != null && item.Timestamp - latestObj.Timestamp < TimeSpan.FromMinutes(1))
                {
                    return;
                }

                BlogPost post = await _blogService.GetPostAsync(item.PostId);
                if (post == null)
                {
                    return;
                }

                _ = await _store.AddAsync(item);
                post.Object.AccessCount += 1;
                _ = await _blogService.UpdatePostAsync(post.Object);

                foreach (BlogPostAccessObject obj in objs.ToList().Where(x => DateTime.Now - x.Timestamp > TimeSpan.FromDays(3)))
                {
                    await _store.DeleteAsync(obj.Id);
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }
    }
}
