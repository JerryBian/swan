using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Infrastructure.Command;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Infrastructure.Git
{
    public class GitClient : IGitClient
    {
        private readonly ICommand _command;
        private readonly ILogger<GitClient> _logger;
        private const string GitHubUserAgent = "laobian";

        public GitClient(ICommand command, ILogger<GitClient> logger)
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

            await _command.ExecuteAsync($"cd {gitConfig.GitCloneToDir}; git config set user.name \"{gitConfig.GitCommitUser}\"; git config user.email \"{gitConfig.GitCommitEmail}\"");
            _logger.LogInformation("Set local commit user completed.");
        }

        public async Task CommitAsync(string workingDir, string message)
        {
            if (!Directory.Exists(workingDir))
            {
                return;
            }

            await _command.ExecuteAsync($"cd {workingDir}; git add .; git commit -m \"{message}\"; git push -u origin");
            _logger.LogInformation("Commit completed.");
        }
    }
}
