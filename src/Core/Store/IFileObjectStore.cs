using Swan.Core.Model.Object;

namespace Swan.Core.Store
{
    public interface IFileObjectStore<T> where T : FileObjectBase
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> AddAsync(T obj);

        Task<T> UpdateAsync(T obj, bool coreUpdate = true);

        Task DeleteAsync(Predicate<T> filter);
    }
}
