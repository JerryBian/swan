using System;
using Laobian.Blog.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Cache
{
    public class CacheClient : ICacheClient
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<CacheClient> _logger;
        private readonly IMemoryCache _memoryCache;

        public CacheClient(IMemoryCache memoryCache, IBlogService blogService, ILogger<CacheClient> logger)
        {
            _logger = logger;
            _blogService = blogService;
            _memoryCache = memoryCache;
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> func)
        {
            return _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                var val = func();
                entry.Value = val;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                entry.ExpirationTokens.Add(new BlogChangeToken(_blogService));

                _logger.LogInformation($"Blog cache created. Key: {cacheKey}.");
                return val;
            });
        }
    }
}