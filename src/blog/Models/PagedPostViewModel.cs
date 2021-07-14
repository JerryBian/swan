using System;
using System.Collections.Generic;
using Laobian.Share.Blog;

namespace Laobian.Blog.Models
{
    public class PagedPostViewModel
    {
        public PagedPostViewModel(int currentPage, int postCount, int postsPerPage)
        {
            if (postCount < 0)
            {
                postCount = 0;
            }

            TotalPages = Convert.ToInt32(Math.Ceiling(postCount / (double)postsPerPage));

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (currentPage > TotalPages)
            {
                currentPage = TotalPages;
            }

            CurrentPage = currentPage;
            Posts = new List<PostViewModel>();
        }

        public int TotalPages { get; }

        public int CurrentPage { get; }

        public List<PostViewModel> Posts { get; }

        public string Url { get; set; }
    }
}
