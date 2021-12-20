using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share
{
    public class PagedViewModel<T>
    {
        public PagedViewModel(int currentPage, int totalCount, int postsPerPage)
        {
            if (totalCount < 0)
            {
                totalCount = 0;
            }

            TotalPages = Convert.ToInt32(Math.Ceiling(totalCount / (double)postsPerPage));

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (currentPage > TotalPages)
            {
                currentPage = TotalPages;
            }

            CurrentPage = currentPage;
            Items = new List<T>();
        }

        public int TotalPages { get; }

        public int CurrentPage { get; }

        public List<T> Items { get; }

        public string Url { get; set; }
    }
}
