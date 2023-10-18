using GitStoreDotnet;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Swan.Core.Helper;
using Swan.Core.Model;

namespace Swan.Core.Store
{
    internal class SwanStore : ISwanStore
    {
        private const string CacheKey = "core.swanstore";

        private readonly IGitStore _gitStore;
        private readonly ILogger<SwanStore> _logger;
        private readonly IMemoryCache _memoryCache;

        public SwanStore(IGitStore gitStore, ILogger<SwanStore> logger, IMemoryCache memoryCahce)
        {
            _logger = logger;
            _gitStore = gitStore;
            _memoryCache = memoryCahce;
        }

        public async Task<StoreObject> GetAsync()
        {
            return await _memoryCache.GetOrCreateAsync(CacheKey, async entry =>
            {
                StoreObject obj = new StoreObject();
                try
                {
                    obj.BlogPosts = JsonHelper.Deserialize<List<BlogPost>>(await _gitStore.GetTextAsync(BlogPost.GitStorePath, true));
                    obj.BlogSeries = JsonHelper.Deserialize<List<BlogSeries>>(await _gitStore.GetTextAsync(BlogSeries.GitStorePath, true));
                    obj.BlogTags = JsonHelper.Deserialize<List<BlogTag>>(await _gitStore.GetTextAsync(BlogTag.GitStorePath, true));

                    PostExtend(obj);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Load swan objects from GitStore failed.");
                }

                return obj;
            });
        }

        public void Clear()
        {
            _memoryCache.Remove(CacheKey);
        }
        private void PostExtend(StoreObject obj)
        {
            obj.BlogPosts ??= new List<BlogPost>();
            obj.BlogSeries ??= new List<BlogSeries>();
            obj.BlogTags ??= new List<BlogTag>();
        }
    }
}
