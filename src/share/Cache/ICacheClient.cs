using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public interface ICacheClient
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> func, IChangeToken changeToken = null);

        T GetOrCreate<T>(string cacheKey, Func<T> func, IChangeToken changeToken = null);
    }
}
