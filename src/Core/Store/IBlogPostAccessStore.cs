using Swan.Core.Model.Object;

namespace Swan.Core.Store
{
    public interface IBlogPostAccessStore
    {
        void Ingest(string postId, string ipAddress);

        List<BlogPostAccessObject> DequeueAll();
    }
}
