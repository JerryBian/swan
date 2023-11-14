using Microsoft.AspNetCore.Http;
using Swan.Core.Model;

namespace Swan.Core.Service
{
    public interface ISwanService
    {
        Task AddAsync<T>(T item) where T : SwanObject;

        Task DeleteAsync<T>(string id) where T : SwanObject;

        Task<List<T>> FindAsync<T>(bool searchAdminStore, Predicate<T> wherePredicate = null) where T : SwanObject;

        Task<List<T>> FindAsync<T>(HttpContext httpContext, Predicate<T> wherePredicate = null) where T : SwanObject;

        Task<T> FindFirstOrDefaultAsync<T>(bool searchAdminStore, Predicate<T> predicate = null) where T : SwanObject;

        Task<T> FindFirstOrDefaultAsync<T>(HttpContext context, Predicate<T> predicate = null) where T : SwanObject;

        Task<T> FindAsync<T>(string id) where T : SwanObject;

        Task UpdateAsync<T>(T item) where T : SwanObject;

        Task<string> UploadFileAsync(string fileName, byte[] binary);
    }
}