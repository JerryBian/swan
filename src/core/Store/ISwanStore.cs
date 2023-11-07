namespace Swan.Core.Store
{
    public interface ISwanStore
    {
        Task AddPageHitAsync(string url, string ip);

        Task<List<string>> GetPageHitsAsync();

        Task AddBlacklistAsync(string ip);

        Task<bool> IsInBlacklistAsync(string ip);
    }
}
