using Swan.Lib.Model;

namespace Swan.Lib.Service
{
    public interface IBlacklistService
    {
        Task UdpateAsync(BlacklistItem item);

        Task DeleteAsync(string ip);

        Task<List<BlacklistItem>> GetAllAsync();
    }
}
