using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Blog.Model
{
    public class BlogArchive
    {
        public BlogArchive(string title)
        {
            Title = title;
            Posts = new List<BlogPost>();
        }

        public string Title { get; set; }

        public List<BlogPost> Posts { get; set; }
    }
}
