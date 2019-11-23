using System.Collections.Generic;

namespace Laobian.Share.Blog.Model
{
    public class BlogTag
    {
        public BlogTag()
        {
            Posts = new List<BlogPost>();
        }

        public string Name { get; set; }

        public string Link { get; set; }

        public List<BlogPost> Posts { get; }
    }
}