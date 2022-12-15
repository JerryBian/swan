using Microsoft.Extensions.Options;
using Swan.Lib.Command;
using Swan.Lib.Extension;
using Swan.Lib.Option;

namespace Swan.HostedServices
{
    public class GitFileHostedService : BackgroundService
    {
        private readonly SwanOption _option;
        private readonly ICommandClient _commandClient;
        private readonly ILogger<GitFileHostedService> _logger;

        public GitFileHostedService(ICommandClient commandClient, IOptions<SwanOption> option, ILogger<GitFileHostedService> logger)
        {
            _commandClient = commandClient;
            _option = option.Value;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_option.SkipGitOperations)
            {
                string dir = GetBaseDir();
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }

                int retryTimes = 0;
                while (retryTimes <= 3 && !Directory.Exists(dir))
                {
                    retryTimes++;
                    string repoUrl =
                        $"https://{_option.GitHubApiToken}@github.com/{_option.GitHubUserName}/{_option.GitHubRepoName}.git";
                    string command = $"git clone -b {_option.GitHubBranchName} --single-branch {repoUrl} {dir}";
                    command += $" && cd {dir}";
                    command += " && git config --local user.name \"swan\"";
                    command += $" && git config --local user.email \"{_option.GitHubUserEmail}\"";
                    if (retryTimes > 1)
                    {
                        _logger.LogInformation($"Retry: {retryTimes}... starting to pull DB repo.");
                    }

                    string output = await _commandClient.RunAsync(command, cancellationToken);
                    if (retryTimes > 1)
                    {
                        _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}");
                    }

                    _logger.LogInformation(output);
                }

                if (!Directory.Exists(dir))
                {
                    _logger.LogError($"Failed to pull git repo to {dir}, app will be termintated.");
                    throw new Exception();
                }

            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                if (!_option.SkipGitOperations && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    try
                    {
                        await GitPushAsync(":alarm_clock: Schedule", stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Scheduled git push failed.");
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_option.SkipGitOperations)
            {
                await GitPushAsync("Stop", cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }

        private async Task GitPushAsync(string message, CancellationToken cancellationToken = default)
        {
            string dir = GetBaseDir();
            if (!Directory.Exists(dir))
            {
                _logger.LogError($"Git repo not exists: {dir}");
                return;
            }

            List<string> commands = new()
            {
            $"cd \"{dir}\"", "git add .",
            $"git commit -m \"{message} [{DateTime.Now.ToTime()}]\"", "git push"
        };
            string command =
                $"{string.Join(" && ", commands)}";
            string output = await _commandClient.RunAsync(command, cancellationToken);
            _logger.LogInformation($"Git push finished: {output}");
        }

        private string GetBaseDir()
        {
            string dir = Path.Combine(_option.AssetLocation, "asset");
            return dir;
        }
    }
}
