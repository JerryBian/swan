using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, ICachePolicy cachePolicy, Func<Task<T>> func)
        {
            if (cachePolicy.NeedExpire())
            {
                await RemoveCacheAsync(cacheKey);
            }

            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                var value = await func();
                cacheEntry.Value = value;
                cacheEntry.AbsoluteExpirationRelativeToNow = cachePolicy.ExpirationRelativeToNow;

                cachePolicy.Cache();
                await _logService.LogInformation($"Cache created. Key: {cacheKey}");

                return value;
            });
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, ICachePolicy cachePolicy, Func<T> func)
        {
            if (cachePolicy.NeedExpire())
            {
                await RemoveCacheAsync(cacheKey);
            }

            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                var value = func();
                cacheEntry.Value = value;
                cacheEntry.AbsoluteExpirationRelativeToNow = cachePolicy.ExpirationRelativeToNow;

                cachePolicy.Cache();
                await _logService.LogInformation($"Cache created. Key: {cacheKey}");

                return value;
            });
        }

        public void ExpireCache(ICachePolicy cachePolicy)
        {
            cachePolicy.Expire();
        }

        private async Task RemoveCacheAsync(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
            await _logService.LogInformation($"Cache removed. Key: {cacheKey}");
        }
    }
}
