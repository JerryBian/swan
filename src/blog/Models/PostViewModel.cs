using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class PostViewModel
    {
        public BlogPost Post { get; set; }

        public BlogPost PrevPost { get; set; }

        public BlogPost NextPost { get; set; }
    }
}