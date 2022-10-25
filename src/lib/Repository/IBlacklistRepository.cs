using Laobian.Lib.Model;

namespace Laobian.Lib.Repository
{
    public interface IBlacklistRepository
    {
        Task AddAsync(BlacklistItem item);

        Task DeleteAsync(BlacklistItem item);

        Task<List<BlacklistItem>> GetAllAsync();
    }
}
