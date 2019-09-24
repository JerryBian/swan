using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Laobian.Share.Infrastructure.Cache
{
    /// <summary>
    /// Implementation for IMemoryCacheClient
    /// </summary>
    public class MemoryCacheClient : IMemoryCacheClient
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Constructor of <see cref="MemoryCacheClient"/>
        /// </summary>
        public MemoryCacheClient()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        #region Implementation of IMemoryCacheClient

        /// <inheritdoc />
        public bool TryGet<T>(string key, out T obj)
        {
            return _memoryCache.TryGetValue(key, out obj);
        }

        /// <inheritdoc />
        public void Set<T>(string key, T obj, TimeSpan expireAfter)
        {
            _memoryCache.Set(key, obj, expireAfter);
        }

        /// <inheritdoc />
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> addFunc, TimeSpan expireAfter)
        {
            return await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                var value = await addFunc();
                entry.Value = value;

                entry.AbsoluteExpirationRelativeToNow = expireAfter;
                return value;
            });
        }

        public T GetOrAdd<T>(string key, Func<T> addFunc, TimeSpan expireAfter)
        {
            return _memoryCache.GetOrCreate(key, entry =>
            {
                var value = addFunc();
                entry.Value = value;
                entry.AbsoluteExpirationRelativeToNow = expireAfter;
                return value;
            });
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        #endregion
    }
}
