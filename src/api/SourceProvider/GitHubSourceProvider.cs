using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Command;
using Laobian.Share;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.SourceProvider
{
    public class GitHubSourceProvider : LocalFileSourceProvider
    {
        private readonly ApiConfig _apiConfig;
        private readonly ICommandClient _commandClient;
        private readonly ILogger<GitHubSourceProvider> _logger;

        public GitHubSourceProvider(IOptions<ApiConfig> apiConfig, ICommandClient commandClient,
            ILogger<GitHubSourceProvider> logger) : base(apiConfig)
        {
            _logger = logger;
            _apiConfig = apiConfig.Value;
            _commandClient = commandClient;
        }

        public override async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(PullBlogPostRepoAsync(cancellationToken), PullDbRepoAsync(cancellationToken));
            await base.LoadAsync(cancellationToken);
        }


        public override async Task PersistentAsync(CancellationToken cancellationToken = default)
        {
            await PushDbRepoAsync("update");
        }

        private async Task PushDbRepoAsync(string message)
        {
            if (!Directory.Exists(_apiConfig.GetDbLocation()))
            {
                _logger.LogWarning("Push DB repo failed, local dir not exist.");
                return;
            }

            var commands = new List<string>
            {
                $"cd {_apiConfig.GetDbLocation()}", "git add .", $"git commit -m \"{message}\"", "git push"
            };
            var command =
                $"{string.Join(" && ", commands)}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        private async Task PullBlogPostRepoAsync(CancellationToken cancellationToken)
        {
            if (Directory.Exists(_apiConfig.GetBlogPostLocation()))
            {
                Directory.Delete(_apiConfig.GetBlogPostLocation(), true);
            }

            var retryTimes = 0;
            while (retryTimes <= 3 && !Directory.Exists(_apiConfig.GetBlogPostLocation()))
            {
                retryTimes++;
                var repoUrl =
                    $"https://{_apiConfig.GitHubBlogPostRepoApiToken}@github.com/{_apiConfig.GitHubBlogPostRepoUserName}/{_apiConfig.GitHubBlogPostRepoName}.git";
                var command =
                    $"git clone -b {_apiConfig.GitHubBlogPostRepoBranchName} --single-branch {repoUrl} {_apiConfig.GetBlogPostLocation()}";
                _logger.LogInformation($"Retry: {retryTimes}... starting to pull Blog Post repo.");
                var output = await _commandClient.RunAsync(command, cancellationToken);
                _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}Output: {output}");
            }
        }

        private async Task PullDbRepoAsync(CancellationToken cancellationToken)
        {
            if (Directory.Exists(_apiConfig.GetDbLocation()))
            {
                Directory.Delete(_apiConfig.GetDbLocation(), true);
            }

            var retryTimes = 0;
            while (retryTimes <= 3 && !Directory.Exists(_apiConfig.GetDbLocation()))
            {
                retryTimes++;
                var repoUrl =
                    $"https://{_apiConfig.GitHubDbRepoApiToken}@github.com/{_apiConfig.GitHubDbRepoUserName}/{_apiConfig.GitHubDbRepoName}.git";
                var command =
                    $"git clone -b {_apiConfig.GitHubDbRepoBranchName} --single-branch {repoUrl} {_apiConfig.GetDbLocation()}";
                command += $" && cd {_apiConfig.GetDbLocation()}";
                command += " && git config --local user.name \"API Server\"";
                command += $" && git config --local user.email \"{_apiConfig.AdminEmail}\"";
                _logger.LogInformation($"Retry: {retryTimes}... starting to pull DB repo.");
                var output = await _commandClient.RunAsync(command, cancellationToken);
                _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}Output: {output}");
            }
        }
    }
}