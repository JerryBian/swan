using Swan.Core.Cache;

namespace Swan.Core.Store
{
    public class BlacklistStore : IBlacklistStore
    {
        private readonly ICacheClient _cacheClient;
        private ILogger<BlacklistStore> _logger;

        public BlacklistStore(ICacheClient cacheClient, ILogger<BlacklistStore> logger)
        {
            _cacheClient = cacheClient;
            _logger = logger;
        }

        public bool Has(string ipAddress)
        {
            return _cacheClient.TryGet<object>(ipAddress, out _);
        }

        public void Add(string ipAddress, TimeSpan? expireAfter = null)
        {
            _cacheClient.Set(ipAddress, new object(), expireAfter);
            _logger.LogWarning($"Added IP {ipAddress} to blacklist store, expire after {expireAfter}.");
        }
    }
}
