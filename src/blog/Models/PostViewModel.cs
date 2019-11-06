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

        public string GetMetadataHtml()
        {
            var results = new List<string>();
            results.Add(GetSimpleMetadataHtml());

            var catHtml = GetCategoryHtml();
            if (!string.IsNullOrEmpty(catHtml))
            {
                results.Add(catHtml);
            }

            var tagHtml = GetTagHtml();
            if (!string.IsNullOrEmpty(tagHtml))
            {
                results.Add(tagHtml);
            }

            return string.Join(" &middot; ", results);
        }

        public string GetSimpleMetadataHtml()
        {
            var results = new List<string>();
            results.Add($"<i class=\"fas fa-calendar-alt\"></i> <span>发表于 {Post.CreateTimeString}</span>");
            results.Add($"<i class=\"fas fa-eye\"></i> <span>{Post.VisitString} 次访问</span>");

            return string.Join(" &middot; ", results);
        }

        public string GetCategoryAndTagHtml()
        {
            var categoryHtml = GetCategoryHtml();
            var tagHtml = GetTagHtml();
            if (!string.IsNullOrEmpty(tagHtml))
            {
                return categoryHtml + " &middot; " + tagHtml;
            }

            return categoryHtml;
        }

        public string GetCategoryHtml()
        {
            var results = new List<string>();

            foreach (var blogCategory in Categories)
            {
                results.Add($"<a href='{blogCategory.GetLink()}' title='{blogCategory.Name}'>{blogCategory.Name}</a>");
            }

            if(!results.Any())
            {
                return string.Empty;
            }

            return $"<i class=\"fas fa-folder\"></i> <span>{string.Join(", ", results)}</span>";
        }

        public string GetTagHtml()
        {
            var results = new List<string>();

            foreach (var tag in Tags)
            {
                results.Add($"<a href='{tag.GetLink()}' title='{tag.Name}'>{tag.Name}</a>");
            }

            if (!results.Any())
            {
                return string.Empty;
            }

            return $"<i class=\"fas fa-tags\"></i> <span>{string.Join(", ", results)}</span>";
        }
    }
}
