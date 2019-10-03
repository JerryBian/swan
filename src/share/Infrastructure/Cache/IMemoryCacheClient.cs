using System;

namespace Laobian.Share.Infrastructure.Cache
{
    /// <summary>
    /// Client for memory cache
    /// </summary>
    public interface IMemoryCacheClient
    {
        /// <summary>
        /// Try get value for specified key
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">The requested key</param>
        /// <param name="obj">Requested value</param>
        /// <returns>True if success, otherwise false</returns>
        bool TryGet<T>(string key, out T obj);

        /// <summary>
        /// Add or replace cache value
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">The cache key</param>
        /// <param name="obj">The cache value</param>
        /// <param name="expireAfter">Automatically expire cache after time range</param>
        void Set<T>(string key, T obj, TimeSpan expireAfter = default);
    }
}
