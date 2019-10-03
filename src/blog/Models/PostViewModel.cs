using System.Collections.Generic;
using System.Linq;
using Laobian.Share.BlogEngine.Model;

namespace Laobian.Blog.Models
{
    public class PostViewModel
    {
        public PostViewModel(BlogPost post)
        {
            Post = post;
        }

        public BlogPost Post { get; set; }

        public List<BlogCategory> Categories { get; } = new List<BlogCategory>();

        public List<BlogTag> Tags { get; } = new List<BlogTag>();

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
            results.Add($"发表于 {Post.CreateTimeString}");
            results.Add($"{Post.VisitString} 次访问");

            return string.Join(" &middot; ", results);
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

            return $"{string.Join(", ", results)}";
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

            return $"{string.Join(", ", results)}";
        }
    }
}
