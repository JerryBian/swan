using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public interface ICacheClient
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, IChangeToken changeToken, Func<Task<T>> func);

        Task<T> GetOrCreateAsync<T>(string cacheKey, IChangeToken changeToken, Func<T> func);
    }
}
