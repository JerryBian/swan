namespace Laobian.Blog.Cache
{
    public static class CacheKeyBuilder
    {
        public static string Build(params object[] parts)
        {
            return $"LAOBIAN:BLOG:{string.Join(":", parts)}";
        }
    }
}