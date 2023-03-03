using Microsoft.Extensions.Caching.Memory;

namespace Swan.Core.Cache
{
    public class MemoryCacheClient : ICacheClient
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheClient(IMemoryCache memoryCache)
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

        public bool TryGet<T>(string key, out T val)
        {
            return _memoryCache.TryGetValue(key, out val);
        }

        public void Set<T>(string key, T val, TimeSpan? expireAfter = null)
        {
            _memoryCache.Set(key, val, expireAfter ?? TimeSpan.MaxValue);
        }

        public void TryRemove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
