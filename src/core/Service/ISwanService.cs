using Microsoft.AspNetCore.Http;
using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ISwanService
    {
        Task AddAsync<T>(T item) where T : SwanObject;

        Task DeleteAsync<T>(string id) where T : SwanObject;

        Task<List<T>> FindPublicAsync<T>(Predicate<T> wherePredicate = null) where T : SwanObject;

        Task<List<T>> FindAsync<T>(Predicate<T> wherePredicate = null, bool adminOnly = false) where T : SwanObject;

        Task<List<T>> FindAsync<T>(HttpContext httpContext, Predicate<T> wherePredicate = null) where T : SwanObject;

        Task<T> FindAsync<T>(string id) where T : SwanObject;

        Task<T> FindFirstOrDefaultAsync<T>(Predicate<T> predicate, bool adminOnly = false) where T : SwanObject;

        Task UpdateAsync<T>(T item) where T : SwanObject;
    }
}