using Swan.Core.Model;

namespace Swan.Core.Store;

public interface ISwanStore
{
    Task AddReadItemAsync(SwanRead swanRead);
    Task<List<SwanRead>> GetReadItemsAsync(bool admin);
    Task LoadAsync();
    Task SaveAsync(string message);
    Task UpdateReadItemAsync(SwanRead swanRead);
}