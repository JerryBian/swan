using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class BlogSeries
    {
        public BlogSeries(BlogSeriesObject obj)
        {
            Object = obj;
        }

        public BlogSeriesObject Object { get; init; }

        public List<BlogPost> Posts { get; } = new();
    }
}
