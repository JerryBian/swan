using System.Threading.Tasks;

namespace Laobian.Share.Git
{
    public interface IGitClient
    {
        Task CloneAsync(GitConfig gitConfig);

        Task CommitAsync(string workingDir, string message);
    }
}
