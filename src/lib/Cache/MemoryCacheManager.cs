using Microsoft.Extensions.Caching.Memory;

namespace Laobian.Lib.Cache
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan? expireAfter = null)
        {
            return await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                T val = await func();
                if (expireAfter != null)
                {
                    entry.AbsoluteExpirationRelativeToNow = expireAfter.Value;
                }

                _ = entry.SetValue(val);
                return val;
            });
        }

        public void TryRemove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
