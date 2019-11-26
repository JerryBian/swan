using System;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public interface ICacheClient
    {
        T GetOrCreate<T>(string cacheKey, Func<T> func, IChangeToken changeToken = null, TimeSpan? expireAfter = null);
    }
}