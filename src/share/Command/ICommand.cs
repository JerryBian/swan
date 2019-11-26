using System.Threading.Tasks;

namespace Laobian.Share.Command
{
    public interface ICommand
    {
        Task ExecuteAsync(string command);
    }
}