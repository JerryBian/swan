using System;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Blog
{
    public class BlogMemoryCacheClient : MemoryCacheClient
    {
        public BlogMemoryCacheClient(ILogger<MemoryCacheClient> logger) : base(logger)
        {
        }

        public override T GetOrCreate<T>(
            string cacheKey,
            Func<T> func,
            IChangeToken changeToken = null,
            TimeSpan? expireAfter = null)
        {
            return MemoryCache.GetOrCreate(cacheKey, cacheEntry =>
            {
                var value = func();
                cacheEntry.Value = value;
                cacheEntry.AbsoluteExpirationRelativeToNow = expireAfter;
                cacheEntry.ExpirationTokens.Add(new BlogAssetChangeToken());

                Logger.LogInformation($"Blog cache created. Key: {cacheKey}.");
                return value;
            });
        }
    }
}