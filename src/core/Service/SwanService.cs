using DotNext.Threading;
using GitStoreDotnet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Swan.Core.Service
{
    internal class SwanService : ISwanService
    {
        private readonly IGitStore _gitStore;
        private readonly IMemoryCache _memoryCache;
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;
        private readonly ConcurrentDictionary<string, object> _cacheKeys;

        public SwanService(IGitStore gitStore, IMemoryCache memoryCache)
        {
            _gitStore = gitStore;
            _memoryCache = memoryCache;
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
            _cacheKeys = new ConcurrentDictionary<string, object>();
        }

        public async Task<List<T>> FindAsync<T>(bool searchAdminStore, Predicate<T> wherePredicate = null) where T : SwanObject
        {
            await _asyncReaderWriterLock.EnterReadLockAsync();

            try
            {
                var storeObject = await GetStoreObjectAsync(searchAdminStore);
                var result = storeObject.Get<T>();
                result = result.
                    Where(x => wherePredicate == null || wherePredicate(x)).
                    ToList();
                return result;
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task<List<T>> FindAsync<T>(HttpContext httpContext, Predicate<T> wherePredicate = null) where T : SwanObject
        {
            return await FindAsync(httpContext.IsAuthorized(), wherePredicate);
        }

        public async Task<T> FindAsync<T>(string id) where T : SwanObject
        {
            return await FindFirstOrDefaultAsync<T>(true, x => StringHelper.EqualsIgoreCase(x.Id, id));
        }

        public async Task<T> FindFirstOrDefaultAsync<T>(bool searchAdminStore, Predicate<T> predicate = null) where T : SwanObject
        {
            var items = await FindAsync<T>(searchAdminStore, x => predicate(x));
            return items.FirstOrDefault();
        }

        public async Task<T> FindFirstOrDefaultAsync<T>(HttpContext context, Predicate<T> predicate = null) where T : SwanObject
        {
            var item = await FindFirstOrDefaultAsync<T>(context.IsAuthorized(), x => predicate(x));
            return item;
        }

        public async Task AddAsync<T>(T item) where T : SwanObject
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                var obj = await GetStoreObjectAsync(true);
                var items = obj.Get<T>();

                item.CreatedAt = item.LastUpdatedAt = DateTime.Now;
                Add(items, item);
                var content = JsonHelper.Serialize(items.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(item.GetGitStorePath(), content);
                ExpireCache();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task UpdateAsync<T>(T item) where T : SwanObject
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                var obj = await GetStoreObjectAsync(true);
                var items = obj.Get<T>();
                var oldObj = items.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, item.Id));
                if (oldObj == null)
                {
                    throw new Exception($"{typeof(T).Name} with id {item.Id} not exists.");
                }

                var newItems = new List<T>(items);
                newItems.Remove(oldObj);
                item.CreatedAt = oldObj.CreatedAt;
                Add(newItems, item);
                var content = JsonHelper.Serialize(newItems.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(item.GetGitStorePath(), content);
                ExpireCache();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        public async Task DeleteAsync<T>(string id) where T : SwanObject
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                var obj = await GetStoreObjectAsync(true);
                var items = obj.Get<T>();
                var oldObj = items.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, id));
                if (oldObj == null)
                {
                    return;
                }

                var newItems = new List<T>(items);
                newItems.Remove(oldObj);

                var content = JsonHelper.Serialize(newItems.OrderByDescending(x => x.CreatedAt));
                await _gitStore.InsertOrUpdateAsync(oldObj.GetGitStorePath(), content);
                ExpireCache();
            }
            finally
            {
                _asyncReaderWriterLock.Release();
            }
        }

        private void Add<T>(List<T> values, T value)
        {
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttribute<JsonPropertyNameAttribute>() == null ||
                    prop.GetCustomAttribute<StoreUniqueAttribute>() == null)
                {
                    continue;
                }

                var val1 = prop.GetValue(value);
                if (values.FirstOrDefault(x => prop.GetValue(x).Equals(val1)) != null)
                {
                    throw new Exception($"{prop.Name} with value {val1} already exists in {typeof(T).Name} list.");
                }
            }

            values.Add(value);
        }

        private async Task<StoreObject> GetStoreObjectAsync(bool adminOnly = false)
        {
            var storeObject = await _memoryCache.GetOrCreateAsync(GetCacheKey<StoreObject>("storeobj", adminOnly.ToString()), async _ =>
            {
                var obj = new StoreObject(adminOnly);
                await obj.PopulateDataAsync(_gitStore);

                return obj;
            });

            return storeObject;
        }

        private string GetCacheKey<T>(params string[] keys)
        {
            var key = $"core.ss.{string.Join(".", keys)}";
            _cacheKeys.TryAdd(key, new object());
            return key;
        }

        private void ExpireCache()
        {
            foreach (var item in _cacheKeys.Keys)
            {
                _memoryCache.Remove(item);
            }
        }
    }
}
