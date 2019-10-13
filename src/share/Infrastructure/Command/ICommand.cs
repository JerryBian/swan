using System.Threading.Tasks;

namespace Laobian.Share.Infrastructure.Command
{
    public interface ICommand
    {
        Task ExecuteAsync(string command);
    }
}
