using System.Collections.Generic;
using Laobian.Share.BlogEngine.Model;

namespace Laobian.Blog.Models
{
    public class PagedPostViewModel
    {
        public Pagination Pagination { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

        public string Url { get; set; }
    }

    public class Pagination
    {
        public Pagination(int currentPage, int totalPages)
        {
            CurrentPage = currentPage;
            if (CurrentPage <= 0) CurrentPage = 1;

            if (CurrentPage > totalPages) CurrentPage = totalPages;

            TotalPages = totalPages;
        }

        public int TotalPages { get; }

        public int CurrentPage { get; }
    }
}
