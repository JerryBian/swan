using System.Collections.Generic;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class PagedPostViewModel
    {
        public PagedPostViewModel(int currentPage, int totalPage)
        {
            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (totalPage <= 0)
            {
                totalPage = 1;
            }

            if (currentPage > totalPage)
            {
                currentPage = totalPage;
            }

            TotalPages = totalPage;
            CurrentPage = currentPage;
            Posts = new List<BlogPost>();
        }

        public int TotalPages { get; }

        public int CurrentPage { get; }

        public List<BlogPost> Posts { get; }

        public string Url { get; set; }
    }
}
