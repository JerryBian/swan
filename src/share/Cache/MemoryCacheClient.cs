using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public class MemoryCacheClient : ICacheClient
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheClient> _logger;

        public MemoryCacheClient(ILogger<MemoryCacheClient> logger)
        {
            _logger = logger;
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> func, IChangeToken changeToken = null)
        {
            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                if (changeToken == null)
                {
                    changeToken = NeverExpireChangeToken.Instance;
                }

                var value = await func();
                cacheEntry.Value = value;
                cacheEntry.ExpirationTokens.Add(changeToken);

                _logger.LogInformation($"Cache created. Key: {cacheKey}");
                return value;
            });
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> func, IChangeToken changeToken = null)
        {
            return _memoryCache.GetOrCreate(cacheKey, cacheEntry =>
            {
                if (changeToken == null)
                {
                    changeToken = NeverExpireChangeToken.Instance;
                }

                var value = func();
                cacheEntry.Value = value;
                cacheEntry.ExpirationTokens.Add(changeToken);

                _logger.LogInformation($"Cache created. Key: {cacheKey}");
                return value;
            });
        }
    }
}
