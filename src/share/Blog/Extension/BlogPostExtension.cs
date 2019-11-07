using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Extension
{
    public static class BlogPostExtension
    {
        public static string GetMetadataHtml(this BlogPost post)
        {
            var results = new List<string>();
            results.Add(post.GetSimpleMetadataHtml());

            var catHtml = post.GetCategoryHtml();
            if (!string.IsNullOrEmpty(catHtml))
            {
                results.Add(catHtml);
            }

            var tagHtml = post.GetTagHtml();
            if (!string.IsNullOrEmpty(tagHtml))
            {
                results.Add(tagHtml);
            }

            return string.Join(" &middot; ", results);
        }

        public static string GetSimpleMetadataHtml(this BlogPost post)
        {
            var results = new List<string>();
            results.Add($"<i class=\"fas fa-calendar-alt\"></i> <span>发表于 {post.PublishTimeString}</span>");
            results.Add($"<i class=\"fas fa-eye\"></i> <span>{post.AccessCountString} 次访问</span>");

            return string.Join(" &middot; ", results);
        }

        public static string GetCategoryAndTagHtml(this BlogPost post)
        {
            var categoryHtml = post.GetCategoryHtml();
            var tagHtml = post.GetTagHtml();
            if (!string.IsNullOrEmpty(tagHtml))
            {
                return categoryHtml + " &middot; " + tagHtml;
            }

            return categoryHtml;
        }

        public static string GetCategoryHtml(this BlogPost post)
        {
            var results = new List<string>();

            foreach (var blogCategory in post.Categories)
            {
                results.Add($"<a href='{blogCategory.GetLink()}' title='{blogCategory.Name}'>{blogCategory.Name}</a>");
            }

            if (!results.Any())
            {
                return string.Empty;
            }

            return $"<i class=\"fas fa-folder\"></i> <span>{string.Join(", ", results)}</span>";
        }

        public static string GetTagHtml(this BlogPost post)
        {
            var results = new List<string>();

            foreach (var tag in post.Tags)
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
