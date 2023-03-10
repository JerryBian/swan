using Swan.Core.Model.Object;
using System.Collections.Concurrent;

namespace Swan.Core.Store
{
    public class BlogPostAccessStore : IBlogPostAccessStore
    {
        private readonly ConcurrentQueue<BlogPostAccessObject> _store;

        public BlogPostAccessStore()
        {
            _store = new ConcurrentQueue<BlogPostAccessObject>();
        }

        public void Ingest(string postId, string ipAddress)
        {
            _store.Enqueue(new()
            {
                PostId = postId,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            });
        }

        public List<BlogPostAccessObject> DequeueAll()
        {
            List<BlogPostAccessObject> items = new();
            while (_store.TryDequeue(out BlogPostAccessObject item))
            {
                items.Add(item);
            }

            return items;
        }
    }
}
