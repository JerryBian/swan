namespace Laobian.Blog
{
    public class CacheKey
    {
        public static string Build(params object[] parts)
        {
            return $"LAOBIAN:BLOG:{string.Join(":", parts)}";
        }
    }
}
