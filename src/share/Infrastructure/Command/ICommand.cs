using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laobian.Share.Infrastructure.Command
{
    public interface ICommand
    {
        Task<List<string>> ExecuteAsync(string command);
    }
}
