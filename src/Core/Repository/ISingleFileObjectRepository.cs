namespace Swan.Core.Repository
{
    public interface ISingleFileObjectRepository<T>
    {
        Task<T> GetAsync(string id);

        Task<List<T>> GetAllAsync();

        Task<T> CreateAsync(T obj);

        Task<T> UpdateAsync(T obj);

        Task DeleteAsync(string id);
    }
}
