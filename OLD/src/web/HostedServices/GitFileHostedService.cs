using GitStoreDotnet;
using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Option;

namespace Swan.Web.HostedServices
{
    public class GitFileHostedService : BackgroundService
    {
        private readonly ILogger<GitFileHostedService> _logger;
        private readonly IGitStore _gitStore;
        private readonly SwanOption _option;

        public GitFileHostedService(ILogger<GitFileHostedService> logger, IGitStore gitStore, IOptions<SwanOption> option)
        {
            _logger = logger;
            _gitStore = gitStore;
            _option = option.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_option.SkipGitOperation)
            {
                await _gitStore.PullFromRemoteAsync(cancellationToken);
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastTimestamp = DateTime.Now;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).OkForCancel();

                if (!_option.SkipGitOperation && DateTime.Now - lastTimestamp > TimeSpan.FromHours(1))
                {
                    try
                    {
                        await _gitStore.PushToRemoteAsync("Schedule push");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to execute {nameof(GitFileHostedService)}.");
                    }
                    finally
                    {
                        lastTimestamp = DateTime.Now;
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_option.SkipGitOperation)
            {
                try
                {
                    await _gitStore.PushToRemoteAsync("OnStop");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to push during stopping.");
                }
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
