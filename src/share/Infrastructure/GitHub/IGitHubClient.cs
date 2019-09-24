using System.Threading.Tasks;

namespace Laobian.Share.Infrastructure.GitHub
{
    public interface IGitHubClient
    {
        Task CloneAsync(GitConfig gitConfig);

        Task CommitPlanTextAsync(GitConfig gitConfig, string filePath, string content, string message);
    }
}
