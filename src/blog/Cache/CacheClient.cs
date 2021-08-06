using System;
using Laobian.Blog.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Cache
{
    public class CacheClient : ICacheClient
    {
        private readonly ILogger<CacheClient> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ISystemData _systemData;

        public CacheClient(IMemoryCache memoryCache, ISystemData systemData, ILogger<CacheClient> logger)
        {
            _logger = logger;
            _systemData = systemData;
            _memoryCache = memoryCache;
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> func, TimeSpan? expireAfter = null)
        {
            return _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                var val = func();
                entry.Value = val;
                entry.AbsoluteExpirationRelativeToNow = expireAfter;
                entry.ExpirationTokens.Add(new BlogChangeToken(_systemData));

                _logger.LogInformation($"Blog cache created. Key: {cacheKey}.");
                return val;
            });
        }
    }
}