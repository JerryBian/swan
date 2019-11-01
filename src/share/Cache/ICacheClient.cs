using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Cache
{
    public interface ICacheClient
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, ICachePolicy cachePolicy, Func<Task<T>> func);

        Task<T> GetOrCreateAsync<T>(string cacheKey, ICachePolicy cachePolicy, Func<T> func);

        void ExpireCache(ICachePolicy cachePolicy);
    }
}
