using System;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Command;
using Laobian.Share.Extension;
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

        public async Task CloneToLocalAsync(GitConfig gitConfig)
        {
            var localPath = Path.GetFullPath(gitConfig.GitCloneToDir);
            var commandText = $"Remove-Item -Path {localPath} -Force -Recurse -ErrorAction SilentlyContinue";
            var repoUrl =
                $"https://{gitConfig.GitHubAccessToken}@github.com/{gitConfig.GitHubRepositoryOwner}/{gitConfig.GitHubRepositoryName}.git";
            commandText += $"; git clone -b {gitConfig.GitHubRepositoryBranch} --single-branch {repoUrl} {localPath}";
            commandText +=
                $"&& cd {gitConfig.GitCloneToDir} && git config --local user.name \"{gitConfig.GitCommitUser}\" && git config --local user.email \"{gitConfig.GitCommitEmail}\"";

            await _command.ExecuteAsync(commandText);
            _logger.LogInformation("Clone repository to local completed.");
        }

        public async Task CommitAsync(string workingDir, string message)
        {
            if (!Directory.Exists(workingDir))
            {
                return;
            }

            await _command.ExecuteAsync(
                $"cd {workingDir} && git add . && git commit -m \"{message}\" && git push");
            _logger.LogInformation($"Commit completed, message = {message}.");
        }
    }
}