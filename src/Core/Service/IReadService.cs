using Swan.Core.Model;
using Swan.Core.Model.Object;

namespace Swan.Core.Service
{
    public interface IReadService
    {
        Task<ReadModel> AddAsync(ReadObject item);
        Task<List<ReadModel>> GetAllAsync(bool isAdmin);
        Task<ReadModel> GetAsync(string id);
        Task<ReadModel> UpdateAsync(ReadObject item);
    }
}