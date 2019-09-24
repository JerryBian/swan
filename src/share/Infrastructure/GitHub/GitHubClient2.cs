using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Infrastructure.Command;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Laobian.Share.Infrastructure.GitHub
{
    public class GitHubClient2 : IGitHubClient
    {
        private readonly ICommand _command;
        private readonly ILogger<GitHubClient2> _logger;
        private const string GitHubUserAgent = "laobian";

        public GitHubClient2(ICommand command, ILogger<GitHubClient2> logger)
        {
            _command = command;
            _logger = logger;
        }

        public async Task CloneAsync(GitConfig gitConfig)
        {
            var localPath = Path.GetFullPath(gitConfig.GitCloneToDir);
            await _command.ExecuteAsync($"Remove-Item -Path {localPath} -Force -Recurse -ErrorAction SilentlyContinue");

            _logger.LogInformation("Clean folder {0} completed.", localPath);

            var repoUrl = $"https://{gitConfig.GitHubAccessToken}@github.com/{gitConfig.GitHubRepositoryOwner}/{gitConfig.GitHubRepositoryName}.git";
            var outputs = await _command.ExecuteAsync(
                $"git clone -b {gitConfig.GitHubRepositoryBranch} --single-branch {repoUrl} {localPath}");
            _logger.LogInformation("Clone assets to local completed.");
        }

        public async Task CommitPlanTextAsync(GitConfig gitConfig, string filePath, string content, string message)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue(GitHubUserAgent))
            {
                Credentials = new Credentials(gitConfig.GitHubAccessToken)
            };

            try
            {
                var existingFile = await gitHubClient.Repository.Content.GetAllContentsByRef(
                    gitConfig.GitHubRepositoryOwner, gitConfig.GitHubRepositoryName, filePath, gitConfig.GitHubRepositoryBranch);
                await gitHubClient.Repository.Content.UpdateFile(
                    gitConfig.GitHubRepositoryOwner,
                    gitConfig.GitHubRepositoryName,
                    filePath,
                    new UpdateFileRequest(message, content, existingFile[0].Sha, gitConfig.GitHubRepositoryBranch));
            }
            catch (NotFoundException)
            {
                await gitHubClient.Repository.Content.CreateFile(
                    gitConfig.GitHubRepositoryOwner,
                    gitConfig.GitHubRepositoryName,
                    filePath,
                    new CreateFileRequest(message, content, gitConfig.GitHubRepositoryBranch));
            }
        }
    }
}
