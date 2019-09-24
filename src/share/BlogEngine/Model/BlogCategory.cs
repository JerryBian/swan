namespace Laobian.Share.BlogEngine.Model
{
    public class BlogCategory
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public string GetLink()
        {
            return $"/category/#{Link}";
        }

        public static BlogCategory Default => new BlogCategory
            {Name = BlogConstant.DefaultCategoryName, Link = BlogConstant.DefaultCategoryLink};
    }
}
