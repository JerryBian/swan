using Swan.Lib.Model;

namespace Swan.Lib.Repository
{
    public interface IBlacklistRepository
    {
        Task UpdateAsync(BlacklistItem item);

        Task DeleteAsync(string ip);

        Task<List<BlacklistItem>> GetAllAsync();
    }
}
