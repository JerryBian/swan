using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class BlogPost
    {
        public BlogPost(BlogPostObject obj)
        {
            Object = obj;
        }

        public BlogPostObject Object { get; init; }
    }
}
