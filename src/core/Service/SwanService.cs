using DotNext.Threading;
using GitStoreDotnet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using System;
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

        public async Task<List<T>> FindAsync<T>(Predicate<T> wherePredicate = null) where T : SwanObject
        {
            await _asyncReaderWriterLock.EnterReadLockAsync();

            try
            {
                var storeObject = await GetStoreObjectAsync();
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
            if (httpContext.IsAuthorized())
            {
                return await FindAsync(wherePredicate);
            }
            else
            {
                return await FindPublicAsync(wherePredicate);
            }
        }

        public async Task<List<T>> FindPublicAsync<T>(Predicate<T> wherePredicate = null) where T : SwanObject
        {
            return await FindAsync<T>(x => x.IsPublicToEveryOne() && (wherePredicate == null || wherePredicate(x)));
        }

        public async Task<T> FindAsync<T>(string id) where T : SwanObject
        {
            return await FindFirstOrDefaultAsync<T>(x => StringHelper.EqualsIgoreCase(x.Id, id));
        }

        public async Task<T> FindFirstOrDefaultAsync<T>(Predicate<T> predicate) where T : SwanObject
        {
            var items = await FindAsync<T>(x => predicate(x));
            return items.FirstOrDefault();
        }

        public async Task AddAsync<T>(T item) where T : SwanObject
        {
            await _asyncReaderWriterLock.EnterWriteLockAsync();

            try
            {
                var obj = await GetStoreObjectAsync();
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
                var obj = await GetStoreObjectAsync();
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
                var obj = await GetStoreObjectAsync();
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
                if (values.FirstOrDefault(x => prop.GetValue(x) == val1) != null)
                {
                    throw new Exception($"{prop.Name} with value {val1} already exists in {typeof(T).Name} list.");
                }
            }
        }

        private async Task<StoreObject> GetStoreObjectAsync()
        {
            var storeObject = await _memoryCache.GetOrCreateAsync(GetCacheKey<StoreObject>("storeobj"), async _ =>
            {
                var obj = new StoreObject();
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
