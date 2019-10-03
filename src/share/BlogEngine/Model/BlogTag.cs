using Laobian.Share.Helper;

namespace Laobian.Share.BlogEngine.Model
{
    public class BlogTag
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public string GetLink()
        {
            return AddressHelper.GetAddress(true, "tag", $"#{Link}");
        }

        public static BlogTag Default => new BlogTag
            {Name = BlogConstant.DefaultTagName, Link = BlogConstant.DefaultTagLink};
    }
}
