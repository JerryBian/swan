using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public class MemoryCacheClient : ICacheClient
    {
        protected readonly ILogger<MemoryCacheClient> Logger;
        protected readonly IMemoryCache MemoryCache;

        public MemoryCacheClient(ILogger<MemoryCacheClient> logger)
        {
            Logger = logger;
            MemoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public virtual T GetOrCreate<T>(string cacheKey, Func<T> func, IChangeToken changeToken = null,
            TimeSpan? expireAfter = null)
        {
            return MemoryCache.GetOrCreate(cacheKey, cacheEntry =>
            {
                var value = func();
                cacheEntry.Value = value;
                cacheEntry.AbsoluteExpirationRelativeToNow = expireAfter;

                if (changeToken != null)
                {
                    cacheEntry.ExpirationTokens.Add(changeToken);
                }

                Logger.LogInformation($"Cache created. Key: {cacheKey}");
                return value;
            });
        }
    }
}