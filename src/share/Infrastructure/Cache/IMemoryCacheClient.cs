using System;
using System.Threading.Tasks;

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
        void Set<T>(string key, T obj, TimeSpan expireAfter);

        /// <summary>
        /// Get cache value by key, if not exist will first set cache then return value
        /// </summary>
        /// <typeparam name="T">Type of cache value</typeparam>
        /// <param name="key">The cache key</param>
        /// <param name="addFunc">Func for adding new cache</param>
        /// <param name="expireAfter">Automatically expire cache after time range</param>
        /// <returns>The cache value</returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> addFunc, TimeSpan expireAfter);

        /// <summary>
        /// Remove cache if exists
        /// </summary>
        /// <param name="key">The cache key</param>
        void Remove(string key);

        T GetOrAdd<T>(string key, Func<T> addFunc, TimeSpan expireAfter);
    }
}
