using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ISwanService
    {
        Task AddAsync<T>(T item) where T : SwanObject;

        Task DeleteAsync<T>(string id) where T : SwanObject;

        Task<List<T>> FindAsync<T>(Predicate<T> predicate = null) where T : SwanObject;

        Task<T> FindAsync<T>(string id) where T : SwanObject;

        Task<T> FindFirstOrDefaultAsync<T>(Predicate<T> predicate) where T : SwanObject;

        Task UpdateAsync<T>(T item) where T : SwanObject;
    }
}