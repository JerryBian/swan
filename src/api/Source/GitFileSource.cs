using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Command;
using Laobian.Share;
using Laobian.Share.Extension;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Source
{
    public class GitFileSource : LocalFileSource
    {
        private readonly ICommandClient _commandClient;
        private readonly LaobianApiOption _laobianApiOption;
        private readonly ILogger<GitFileSource> _logger;
        private bool _prepared;

        public GitFileSource(IOptions<LaobianApiOption> apiOption, ILogger<GitFileSource> logger,
            ICommandClient commandClient) : base(apiOption, logger)
        {
            _logger = logger;
            _commandClient = commandClient;
            _laobianApiOption = apiOption.Value;
        }

        public override async Task PrepareAsync(CancellationToken cancellationToken = default)
        {
            if (_prepared)
            {
                return;
            }

            await PullDbRepoAsync(cancellationToken);
            await base.PrepareAsync(cancellationToken);
            _prepared = true;
        }

        public override async Task FlushAsync(string message)
        {
            FileLocker.WaitOne();
            try
            {
                await base.FlushAsync(message);
                await PushDbRepoAsync(message);
            }
            finally
            {
                FileLocker.Set();
            }
        }

        private async Task PushDbRepoAsync(string message)
        {
            var assetDbFolder = Path.Combine(_laobianApiOption.AssetLocation, Constants.AssetDbFolder);
            if (!Directory.Exists(assetDbFolder))
            {
                _logger.LogWarning($"Push DB repo failed, local dir not exist: {assetDbFolder}.");
                return;
            }

            var commands = new List<string>
            {
                $"cd \"{assetDbFolder}\"", "git add .",
                $"git commit -m \"{message} [{DateTime.Now.ToTime()}]\"", "git push"
            };
            var command =
                $"{string.Join(" && ", commands)}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        private async Task PullDbRepoAsync(CancellationToken cancellationToken)
        {
            var assetDbFolder = Path.Combine(_laobianApiOption.AssetLocation, Constants.AssetDbFolder);
            if (Directory.Exists(assetDbFolder))
            {
                Directory.Delete(assetDbFolder, true);
            }

            var retryTimes = 0;
            while (retryTimes <= 3 && !Directory.Exists(assetDbFolder))
            {
                retryTimes++;
                var repoUrl =
                    $"https://{_laobianApiOption.GitHubDbRepoApiToken}@github.com/{_laobianApiOption.GitHubDbRepoUserName}/{_laobianApiOption.GitHubDbRepoName}.git";
                var command =
                    $"git clone -b {_laobianApiOption.GitHubDbRepoBranchName} --single-branch {repoUrl} {assetDbFolder}";
                command += $" && cd {assetDbFolder}";
                command += " && git config --local user.name \"API Server\"";
                command += $" && git config --local user.email \"{_laobianApiOption.AdminEmail}\"";
                if (retryTimes > 1)
                {
                    _logger.LogInformation($"Retry: {retryTimes}... starting to pull DB repo.");
                }

                var output = await _commandClient.RunAsync(command, cancellationToken);
                if (retryTimes > 1)
                {
                    _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}");
                }

                _logger.LogInformation(output);
            }

            if (!Directory.Exists(assetDbFolder))
            {
                _logger.LogError("Pull DB repo failed.");
            }
        }
    }
}