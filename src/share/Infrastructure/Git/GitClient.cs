using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Infrastructure.Command;
using Laobian.Share.Log;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Infrastructure.Git
{
    public class GitClient : IGitClient
    {
        private readonly ICommand _command;
        private readonly ILogService _logService;
        private const string GitHubUserAgent = "laobian";

        public GitClient(ICommand command, ILogService logService)
        {
            _command = command;
            _logService = logService;
        }

        public async Task CloneAsync(GitConfig gitConfig)
        {
            var localPath = Path.GetFullPath(gitConfig.GitCloneToDir);
            await _command.ExecuteAsync($"Remove-Item -Path {localPath} -Force -Recurse -ErrorAction SilentlyContinue");

            await _logService.LogInformation($"Clean folder {localPath} completed.");

            var repoUrl = $"https://{gitConfig.GitHubAccessToken}@github.com/{gitConfig.GitHubRepositoryOwner}/{gitConfig.GitHubRepositoryName}.git";
            await _command.ExecuteAsync(
                $"git clone -b {gitConfig.GitHubRepositoryBranch} --single-branch {repoUrl} {localPath}");
            await _logService.LogInformation("Clone assets to local completed.");

            await _command.ExecuteAsync($"cd {gitConfig.GitCloneToDir}; git config user.name \"{gitConfig.GitCommitUser}\"; git config user.email \"{gitConfig.GitCommitEmail}\"");
            await _logService.LogInformation("Set local commit user completed.");
        }

        public async Task CommitAsync(string workingDir, string message)
        {
            if (!Directory.Exists(workingDir))
            {
                return;
            }

            await _command.ExecuteAsync($"cd {workingDir}; git add .; git commit -m \"{message}\"; git push -u origin");
            await _logService.LogInformation("Commit completed.");
        }
    }
}
