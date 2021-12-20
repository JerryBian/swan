using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Api.Service
{
    public interface IRawFileService
    {
        Task<string> AddRawFileAsync(string fileName, byte[] content,
            CancellationToken cancellationToken = default);
    }
}
