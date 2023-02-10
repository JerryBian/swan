using Swan.Core.Model;
using Swan.Core.Model.Object;

namespace Swan.Core.Service
{
    public interface IReadService
    {
        Task<List<ReadModel>> GetAllAsync();

        Task<ReadModel> GetAsync(string id);

        Task AddAsync(ReadObject item);

        Task UpdateAsync(ReadObject item);
    }
}
