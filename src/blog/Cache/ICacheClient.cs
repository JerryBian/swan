using System;
using Microsoft.Extensions.Primitives;

namespace Laobian.Blog.Cache
{
    public interface ICacheClient
    {
        T GetOrCreate<T>(string cacheKey, Func<T> func, TimeSpan? expireAfter = null);
    }
}
