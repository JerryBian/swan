using System;
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
        public void Set<T>(string key, T obj, TimeSpan expireAfter = default)
        {
            if (expireAfter == default)
            {
                _memoryCache.Set(key, obj);
                return;
            }

            _memoryCache.Set(key, obj, expireAfter);
        }

        #endregion
    }
}
