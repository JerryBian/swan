using Laobian.Share.Blog.Model;

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
    }
}
