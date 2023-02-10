using Swan.Core.Model.Object;

namespace Swan.Core.Repository
{
    public interface IMultipleFileObjectRepository<T> where T : FileObjectBase
    {
        Task<T> GetAsync(string id);

        Task<List<T>> GetAllAsync();

        Task<T> CreateAsync(T obj);

        Task<T> UpdateAsync(T obj);

        Task DeleteAsync(string path);
    }
}
