using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Blog.Models
{
    public class PostArchiveViewModel
    {
        public string Link { get; set; }

        public int Count { get; set; }

        public string Name { get; set; }

        public string BaseUrl { get; set; }

        public List<BlogPost> Posts { get; set; }
    }
}
