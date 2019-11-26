using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Extension
{
    public static class BlogArchiveExtension
    {
        public static string GetRelativeLink(this BlogArchive archive)
        {
            return $"#{archive.Year}/";
        }
    }
}