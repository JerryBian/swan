using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Extension
{
    public static class BlogTagExtension
    {
        public static string GetLink(this BlogTag tag)
        {
            return $"/tag/#{tag.Link}/";
        }

        public static string GetRelativeLink(this BlogTag tag)
        {
            return $"#{tag.Link}/";
        }
    }
}
