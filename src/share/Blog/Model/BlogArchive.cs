using System.Collections.Generic;

namespace Laobian.Share.Blog.Model
{
    public class BlogArchive
    {
        public BlogArchive(int year)
        {
            Title = $"{year} 年";
            Posts = new List<BlogPost>();
        }

        public string Title { get; set; }

        public int Year { get; set; }

        public List<BlogPost> Posts { get; set; }
    }
}
