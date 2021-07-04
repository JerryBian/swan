using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Command.Laobian.Share.Command;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.SourceProvider
{
    public class GitHubSourceProvider : LocalFileSourceProvider
    {
        private readonly ApiConfig _apiConfig;
        private readonly ICommandClient _commandClient;
        private readonly ILogger<GitHubSourceProvider> _logger;

        public GitHubSourceProvider(IOptions<ApiConfig> apiConfig, ICommandClient commandClient, ILogger<GitHubSourceProvider> logger) : base(apiConfig)
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

        public override async Task SaveTagsAsync(string tags, CancellationToken cancellationToken = default)
        {
            await base.SaveTagsAsync(tags, cancellationToken);
            await PushDbRepoAsync("Update tags");
        }

        private async Task PushDbRepoAsync(string message)
        {
            if (string.IsNullOrEmpty(_apiConfig.DbLocation))
            {
                _logger.LogWarning("Push DB repo failed, local dir not setup.");
                return;
            }

            if (!Directory.Exists(_apiConfig.DbLocation))
            {
                _logger.LogWarning("Push DB repo failed, local dir not exist.");
                return;
            }

            var commands = new List<string>
            {
                $"cd {_apiConfig.DbLocation}", "git add .", $"git commit -m \"{message}\"", "git push"
            };
            var command =
                $"{string.Join(" && ", commands)}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        private async Task PullBlogPostRepoAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_apiConfig.BlogPostLocation))
            {
                throw new LaobianConfigException(_apiConfig.BlogPostLocation);
            }

            if (Directory.Exists(_apiConfig.BlogPostLocation))
            {
                Directory.Delete(_apiConfig.BlogPostLocation, true);
            }

            var repoUrl =
                $"https://{_apiConfig.GitHubBlogPostRepoApiToken}@github.com/{_apiConfig.GitHubBlogPostRepoUserName}/{_apiConfig.GitHubBlogPostRepoName}.git";
            var command =
                $"git clone -b {_apiConfig.GitHubBlogPostRepoBranchName} --single-branch {repoUrl} {_apiConfig.BlogPostLocation}";
            var output = await _commandClient.RunAsync(command, cancellationToken);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        private async Task PullDbRepoAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_apiConfig.DbLocation))
            {
                throw new LaobianConfigException(_apiConfig.DbLocation);
            }

            if (Directory.Exists(_apiConfig.DbLocation))
            {
                Directory.Delete(_apiConfig.DbLocation, true);
            }

            var repoUrl =
                $"https://{_apiConfig.GitHubDbRepoApiToken}@github.com/{_apiConfig.GitHubDbRepoUserName}/{_apiConfig.GitHubDbRepoName}.git";
            var command =
                $"git clone -b {_apiConfig.GitHubDbRepoBranchName} --single-branch {repoUrl} {_apiConfig.DbLocation}";
            var output = await _commandClient.RunAsync(command, cancellationToken);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }
    }
}
