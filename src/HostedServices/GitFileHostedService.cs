using Microsoft.Extensions.Options;
using Swan.Core.Command;
using Swan.Core.Extension;
using Swan.Core.Option;
using Swan.Core.Service;

namespace Swan.HostedServices
{
    public class GitFileHostedService : BackgroundService
    {
        private readonly SwanOption _option;
        private readonly ILogService _logService;
        private readonly ICommandClient _commandClient;
        private readonly ILogger<GitFileHostedService> _logger;

        public GitFileHostedService(
            ICommandClient commandClient, 
            IOptions<SwanOption> option, 
            ILogService logService,
            ILogger<GitFileHostedService> logger)
        {
            _commandClient = commandClient;
            _option = option.Value;
            _logger = logger;
            _logService = logService;
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

            _logService.Start();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken).OkForCancel();

                if (!_option.SkipGitOperations)
                {
                    try
                    {
                        await GitPushAsync(":alarm_clock: Schedule");
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
                await GitPushAsync("Stop");
            }

            await base.StopAsync(cancellationToken);
        }

        private async Task GitPushAsync(string message)
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
            string output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"Git push finished: {output}");
        }

        private string GetBaseDir()
        {
            string dir = Path.Combine(_option.AssetLocation, "asset");
            return dir;
        }
    }
}
