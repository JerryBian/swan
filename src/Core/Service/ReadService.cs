using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class ReadService : IReadService
    {
        private readonly IMemoryObjectStore _store;

        public ReadService(IMemoryObjectStore store)
        {
            _store = store;
        }

        public async Task<List<ReadModel>> GetAllAsync(bool isAdmin)
        {
            var readModels = await _store.GetReadModelsAsync(isAdmin);
            return readModels.OrderByDescending(x => x.Object.CreateTime).ToList();
        }

        public async Task<ReadModel> GetAsync(string id)
        {
            var result = await _store.GetReadModelsAsync(true);
            return result.FirstOrDefault(x => x.Object.Id == id);
        }

        public async Task<ReadModel> AddAsync(ReadObject item)
        {
            return await _store.AddReadAsync(item);
        }

        public async Task<ReadModel> UpdateAsync(ReadObject item)
        {
            return await _store.UpdateReadAsync(item);
        }
    }
}
