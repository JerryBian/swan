using System.Collections.Generic;
using Laobian.Share.Blog.Model;
using Laobian.Share.Helper;

namespace Laobian.Share.Blog.Extension
{
    public static class BlogCategoryExtension
    {
        public static string GetLink(this BlogCategory category)
        {
            return $"/category/#{category.Link}/";
        }

        public static string GetRelativeLink(this BlogCategory category)
        {
            return $"#{category.Link}/";
        }

        public static void Resolve(this BlogCategory category, List<BlogPost> allPosts)
        {
            category.Posts.Clear();
            if (allPosts == null)
            {
                return;
            }

            foreach (var post in allPosts)
            {
                foreach (var categoryName in post.Raw.Category)
                {
                    if (CompareHelper.IgnoreCase(categoryName, category.Name))
                    {
                        category.Posts.Add(post);
                        break;
                    }
                }
            }
        }
    }
}
