namespace Laobian.Lib.Cache
{
    public interface ICacheManager
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan? expireAfter = null);

        void TryRemove(string key);
    }
}
