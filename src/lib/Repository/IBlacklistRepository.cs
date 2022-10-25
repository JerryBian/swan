using Laobian.Lib.Model;

namespace Laobian.Lib.Repository
{
    public interface IBlacklistRepository
    {
        Task UpdateAsync(BlacklistItem item);

        Task DeleteAsync(string ip);

        Task<List<BlacklistItem>> GetAllAsync();
    }
}
