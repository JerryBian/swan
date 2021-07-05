using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Share.Command
{
    namespace Laobian.Share.Command
    {
        public interface ICommandClient
        {
            Task<string> RunAsync(string command, CancellationToken cancellationToken = default);
        }
    }
}