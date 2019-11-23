using System.Collections.Generic;

namespace Laobian.Share.Blog.Model
{
    public class BlogCategory
    {
        public BlogCategory()
        {
            Posts = new List<BlogPost>();
        }

        public string Name { get; set; }

        public string Link { get; set; }

        public List<BlogPost> Posts { get; }
    }
}