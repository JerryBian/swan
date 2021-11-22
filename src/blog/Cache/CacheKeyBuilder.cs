namespace Laobian.Blog.Cache;

public static class CacheKeyBuilder
{
    public static string Build(params object[] parts)
    {
        return $"_c_:{string.Join(":", parts)}";
    }
}