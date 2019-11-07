namespace Laobian.Share.Blog
{
    public class BlogCacheKey
    {
        public static string Build(params object[] parts)
        {
            return $"LAOBIAN:BLOG:{string.Join(":", parts)}";
        }
    }
}
