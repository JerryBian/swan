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
        private readonly ApiOption _apiOption;
        private readonly ICommandClient _commandClient;
        private readonly ILogger<GitHubSourceProvider> _logger;

        public GitHubSourceProvider(IOptions<ApiOption> apiConfig, ICommandClient commandClient,
            ILogger<GitHubSourceProvider> logger) : base(apiConfig)
        {
            _logger = logger;
            _apiOption = apiConfig.Value;
            _commandClient = commandClient;
        }

        public override async Task LoadAsync(bool init = true, CancellationToken cancellationToken = default)
        {
            if (init)
            {
                await Task.WhenAll(PullBlogPostRepoAsync(cancellationToken), PullDbRepoAsync(cancellationToken));
            }

            await base.LoadAsync(init, cancellationToken);
        }


        public override async Task PersistentAsync(CancellationToken cancellationToken = default)
        {
            await PushDbRepoAsync("update");
        }

        private async Task PushDbRepoAsync(string message)
        {
            if (!Directory.Exists(_apiOption.GetDbLocation()))
            {
                _logger.LogWarning("Push DB repo failed, local dir not exist.");
                return;
            }

            var commands = new List<string>
            {
                $"cd {_apiOption.GetDbLocation()}", "git add .", $"git commit -m \"{message}\"", "git push"
            };
            var command =
                $"{string.Join(" && ", commands)}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        private async Task PullBlogPostRepoAsync(CancellationToken cancellationToken)
        {
            if (Directory.Exists(_apiOption.GetBlogPostLocation()))
            {
                Directory.Delete(_apiOption.GetBlogPostLocation(), true);
            }

            var retryTimes = 0;
            while (retryTimes <= 3 && !Directory.Exists(_apiOption.GetBlogPostLocation()))
            {
                retryTimes++;
                var repoUrl =
                    $"https://{_apiOption.GitHubBlogPostRepoApiToken}@github.com/{_apiOption.GitHubBlogPostRepoUserName}/{_apiOption.GitHubBlogPostRepoName}.git";
                var command =
                    $"git clone -b {_apiOption.GitHubBlogPostRepoBranchName} --single-branch {repoUrl} {Path.Combine(_apiOption.AssetLocation, Constants.BlogPostAssetFolder)}";
                _logger.LogInformation($"Retry: {retryTimes}... starting to pull Blog Post repo.");
                var output = await _commandClient.RunAsync(command, cancellationToken);
                _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}Output: {output}");
            }
        }

        private async Task PullDbRepoAsync(CancellationToken cancellationToken)
        {
            if (Directory.Exists(_apiOption.GetDbLocation()))
            {
                Directory.Delete(_apiOption.GetDbLocation(), true);
            }

            var retryTimes = 0;
            while (retryTimes <= 3 && !Directory.Exists(_apiOption.GetDbLocation()))
            {
                retryTimes++;
                var repoUrl =
                    $"https://{_apiOption.GitHubDbRepoApiToken}@github.com/{_apiOption.GitHubDbRepoUserName}/{_apiOption.GitHubDbRepoName}.git";
                var command =
                    $"git clone -b {_apiOption.GitHubDbRepoBranchName} --single-branch {repoUrl} {_apiOption.GetDbLocation()}";
                command += $" && cd {_apiOption.GetDbLocation()}";
                command += " && git config --local user.name \"API Server\"";
                command += $" && git config --local user.email \"{_apiOption.AdminEmail}\"";
                _logger.LogInformation($"Retry: {retryTimes}... starting to pull DB repo.");
                var output = await _commandClient.RunAsync(command, cancellationToken);
                _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}Output: {output}");
            }
        }
    }
}