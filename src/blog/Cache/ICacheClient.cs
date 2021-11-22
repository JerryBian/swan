using System;

namespace Laobian.Blog.Cache;

public interface ICacheClient
{
    T GetOrCreate<T>(string cacheKey, Func<T> func);
}