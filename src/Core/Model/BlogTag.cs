using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class BlogTag
    {
        public BlogTag(BlogTagObject obj)
        {
            Object = obj;
        }

        public BlogTagObject Object { get; init; }

        public List<BlogPost> Posts { get; } = new();
    }
}
