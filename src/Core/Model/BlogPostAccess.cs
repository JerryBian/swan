using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class BlogPostAccess
    {
        public BlogPostAccess(string postId, string ipAddress)
        {
            Object = new BlogPostAccessObject
            {
                PostId = postId,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
        }

        public BlogPostAccessObject Object { get; init; }
    }
}
