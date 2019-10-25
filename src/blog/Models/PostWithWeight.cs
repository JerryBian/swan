using Laobian.Share.BlogEngine.Model;

namespace Laobian.Blog.Models
{
    public class PostWithWeight
    {
        public BlogPost Post { get; set; }

        public double TicksDiff { get; set; }

        public double Weight { get; set; }
    }
}
