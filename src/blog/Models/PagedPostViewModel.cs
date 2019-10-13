using System.Collections.Generic;

namespace Laobian.Blog.Models
{
    public class PagedPostViewModel
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

        public string Url { get; set; }
    }
}
