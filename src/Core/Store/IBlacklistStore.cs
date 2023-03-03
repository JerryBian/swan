namespace Swan.Core.Store
{
    public interface IBlacklistStore
    {
        void Add(string ipAddress, TimeSpan? expireAfter = null);

        bool Has(string ipAddress);
    }
}