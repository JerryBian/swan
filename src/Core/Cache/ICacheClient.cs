namespace Swan.Core.Cache
{
    public interface ICacheClient
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan? expireAfter = null);

        void TryRemove(string key);

        bool TryGet<T>(string key, out T val);

        void Set<T>(string key, T val, TimeSpan? expireAfter = null);
    }
}
