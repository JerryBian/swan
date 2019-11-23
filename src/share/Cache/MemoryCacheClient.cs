using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public class MemoryCacheClient : ICacheClient
    {
        private readonly ILogger<MemoryCacheClient> _logger;
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheClient(ILogger<MemoryCacheClient> logger)
        {
            _logger = logger;
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> func, IChangeToken changeToken = null,
            TimeSpan? expireAfter = null)
        {
            return _memoryCache.GetOrCreate(cacheKey, cacheEntry =>
            {
                var value = func();
                cacheEntry.Value = value;
                cacheEntry.AbsoluteExpirationRelativeToNow = expireAfter;

                if (changeToken != null)
                {
                    cacheEntry.ExpirationTokens.Add(changeToken);
                }

                _logger.LogInformation($"Cache created. Key: {cacheKey}");
                return value;
            });
        }
    }
}