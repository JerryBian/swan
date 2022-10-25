using Laobian.Lib.Cache;
using Laobian.Lib.Model;
using Laobian.Lib.Repository;
using System.Net;

namespace Laobian.Lib.Service
{
    public class BlacklistService : IBlacklistService
    {
        private const string CacheKey = "blacklist";
        private readonly ICacheManager _cacheManager;
        private readonly IBlacklistRepository _repository;

        public BlacklistService(ICacheManager cacheManager, IBlacklistRepository repository)
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }

        public async Task UdpateAsync(BlacklistItem item)
        {
            await _repository.UpdateAsync(item);
            _cacheManager.TryRemove(CacheKey);
        }

        public async Task DeleteAsync(string ip)
        {
            await _repository.DeleteAsync(ip);
            _cacheManager.TryRemove(CacheKey);
        }

        public async Task<List<BlacklistItem>> GetAllAsync()
        {
            return await _cacheManager.GetOrCreateAsync(CacheKey, async () =>
            {
                List<BlacklistItem> result = await _repository.GetAllAsync();
                foreach (BlacklistItem item in result)
                {
                    item.IpBytes = IPAddress.Parse(item.Ip).GetAddressBytes();
                }

                return result;
            });
        }
    }
}
