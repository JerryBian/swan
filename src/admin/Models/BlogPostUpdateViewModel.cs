using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Admin.Models
{
    public class BlogPostUpdateViewModel
    {
        public BlogPost Post { get; set; }

        public List<BlogTag> Tags { get; } = new List<BlogTag>();
    }
}
