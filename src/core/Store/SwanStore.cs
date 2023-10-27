using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swan.Core.Store
{
    internal class SwanStore : ISwanStore
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentQueue<string> _pageHitQueue;

        public SwanStore(IMemoryCache cache)
        {
            _cache = cache;
            _pageHitQueue = new ConcurrentQueue<string>();
        }

        public async Task AddPageHitAsync(string url, string ip)
        {
            var cacheKey = $"core.ss.page.hit.{url}.{ip}";
            await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                _pageHitQueue.Enqueue(url);
                return await Task.FromResult(true);
            });
        }

        public async Task<List<string>> GetPageHitsAsync()
        {
            var result = new List<string>();
            while(_pageHitQueue.TryDequeue(out var url))
            {
                result.Add(url);
            }

            return await Task.FromResult(result);
        }
    }
}
