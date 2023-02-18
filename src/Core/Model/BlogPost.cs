using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class BlogPost
    {
        public BlogPost(BlogPostObject obj)
        {
            Object = obj;
            BlogTags = new List<BlogTag>();
        }

        public BlogPostObject Object { get; init; }

        public List<BlogTag> BlogTags { get; init; }

        public BlogSeries BlogSeries { get; init; }

        public string HtmlContent { get; set; }
    }
}
