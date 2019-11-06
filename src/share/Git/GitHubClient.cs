using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Command;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Git
{
    public class GitHubClient : IGitClient
    {
        private readonly ICommand _command;
        private readonly ILogger<GitHubClient> _logger;

        public GitHubClient(ICommand command, ILogger<GitHubClient> logger)
        {
            _command = command;
            _logger = logger;
        }

        public async Task CloneAsync(GitConfig gitConfig)
        {
            var localPath = Path.GetFullPath(gitConfig.GitCloneToDir);
            await _command.ExecuteAsync($"Remove-Item -Path {localPath} -Force -Recurse -ErrorAction SilentlyContinue");
            _logger.LogInformation($"Clean folder {localPath} completed.");

            var repoUrl = $"https://{gitConfig.GitHubAccessToken}@github.com/{gitConfig.GitHubRepositoryOwner}/{gitConfig.GitHubRepositoryName}.git";
            await _command.ExecuteAsync($"git clone -b {gitConfig.GitHubRepositoryBranch} --single-branch {repoUrl} {localPath}");
            _logger.LogInformation("Clone assets to local completed.");

            await _command.ExecuteAsync($"cd {gitConfig.GitCloneToDir}; git config user.name \"{gitConfig.GitCommitUser}\"; git config user.email \"{gitConfig.GitCommitEmail}\"");
            _logger.LogInformation("Set local commit user completed.");
        }

        public async Task CommitAsync(string workingDir, string message)
        {
            if (!Directory.Exists(workingDir))
            {
                return;
            }

            await _command.ExecuteAsync($"cd {workingDir}; git add .; git commit -m \"{message}\"; git push -u origin");
            _logger.LogInformation($"Commit completed, message = {message}.");
        }
    }
}
