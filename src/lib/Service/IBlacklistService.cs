using Laobian.Lib.Model;

namespace Laobian.Lib.Service
{
    public interface IBlacklistService
    {
        Task UdpateAsync(BlacklistItem item);

        Task DeleteAsync(string ip);

        Task<List<BlacklistItem>> GetAllAsync();
    }
}
