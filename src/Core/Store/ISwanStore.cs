namespace Swan.Core.Store
{
    internal interface ISwanStore
    {
        Task<StoreObject> GetAsync();

        void Clear();
    }
}
