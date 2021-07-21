using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Api.Command
{
    public interface ICommandClient
    {
        Task<string> RunAsync(string command, CancellationToken cancellationToken = default);
    }
}