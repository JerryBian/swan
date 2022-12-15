namespace Swan.Areas.Admin.Models
{
    public class IndexViewModel
    {
        public int BlogPostTotal { get; set; }

        public int BlogPostPublic { get; set; }

        public int BlogPostPrivate { get; set; }

        public int BlogPostVisitTotal { get; set; }

        public int ReadItemTotal { get; set; }

        public int ReadItemPublic { get; set; }

        public int ReadItemPrivate { get; set; }
    }
}
