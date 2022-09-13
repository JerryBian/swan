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

        public bool IsPublished()
        {
            return Raw.IsPublic && Raw.PublishTime >= DateTime.Now;
        }
    }
}
