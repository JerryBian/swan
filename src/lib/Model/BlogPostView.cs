namespace Laobian.Lib.Model
{
    public class BlogPostView
    {
        public BlogPostView(BlogPost raw)
        {
            Raw = raw;
        }

        public BlogPost Raw { get; set; }

        public string HtmlContent { get; set; }

        public string FullLink { get; set; }

        public string Metadata { get; set; }

        public string ExcerptText { get; set; }

        public bool IsPublishedNow => Raw.IsPublic && Raw.PublishTime <= DateTime.Now;
    }
}
