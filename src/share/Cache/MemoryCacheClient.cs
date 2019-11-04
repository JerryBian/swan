using System;
using System.Threading.Tasks;
using Laobian.Share.Log;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public class MemoryCacheClient : ICacheClient
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogService _logService;

        public MemoryCacheClient(ILogService logService)
        {
            _logService = logService;
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, IChangeToken changeToken, Func<Task<T>> func)
        {
            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                var value = await func();
                cacheEntry.Value = value;
                cacheEntry.ExpirationTokens.Add(changeToken);

                await _logService.LogInformation($"Cache created. Key: {cacheKey}");
                return value;
            });
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, IChangeToken changeToken, Func<T> func)
        {
            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                var value = func();
                cacheEntry.Value = value;
                cacheEntry.ExpirationTokens.Add(changeToken);

                await _logService.LogInformation($"Cache created. Key: {cacheKey}");
                return value;
            });
        }

        private async Task RemoveCacheAsync(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
            await _logService.LogInformation($"Cache removed. Key: {cacheKey}");
        }
    }
}
