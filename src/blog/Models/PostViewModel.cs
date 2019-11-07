using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class PostViewModel
    {
        public PostViewModel(BlogPost post)
        {
            Post = post;
        }

        public BlogPost Post { get; set; }

        
    }
}
