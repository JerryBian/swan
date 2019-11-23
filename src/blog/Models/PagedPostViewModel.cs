using System;
using System.Collections.Generic;
using Laobian.Share;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class PagedPostViewModel
    {
        public PagedPostViewModel(int currentPage, int postCount)
        {
            if (postCount < 0)
            {
                postCount = 0;
            }

            TotalPages = Convert.ToInt32(Math.Ceiling(postCount / (double) Global.Config.Blog.PostsPerPage));

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (currentPage > TotalPages)
            {
                currentPage = TotalPages;
            }

            CurrentPage = currentPage;
            Posts = new List<BlogPost>();
        }

        public int TotalPages { get; }

        public int CurrentPage { get; }

        public List<BlogPost> Posts { get; }

        public string Url { get; set; }
    }
}