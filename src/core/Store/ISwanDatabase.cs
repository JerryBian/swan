using Swan.Core.Model;

namespace Swan.Core.Store
{
    public interface ISwanDatabase
    {
        Task StartAsync();

        Task<long?> InsertAsync<T>(T obj) where T : ISwanObject;

        Task UpdateAsync<T>(T obj) where T : ISwanObject;

        Task<List<T>> QueryAsync<T>(DatabaseQuery queryProperties = null) where T : ISwanObject;

        Task<T> QueryFirstOrDefaultAsync<T>(DatabaseQuery queryProperties = null) where T : ISwanObject;

        Task DeleteAsync<T>(long id) where T : ISwanObject;
    }
}
