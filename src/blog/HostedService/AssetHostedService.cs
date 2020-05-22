using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Git;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.HostedService
{
    public class AssetHostedService : BackgroundService
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<AssetHostedService> _logger;

        private DateTime _lastUpdateTime;

        public AssetHostedService(IBlogService blogService, ILogger<AssetHostedService> logger)
        {
            _logger = logger;
            _blogService = blogService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                if (Global.Environment.IsDevelopment())
                {
                    await ExecuteInternalAsync(GitCommitMessageFactory.ScheduleUpdated());
                    _lastUpdateTime = DateTime.Now;
                    _logger.LogInformation("Asset updated. Last executed at= {0}.", _lastUpdateTime);
                }
                else
                {
                    if (DateTime.Now.Hour == Global.Config.Blog.AssetUpdateAtHour &&
                        _lastUpdateTime.Date < DateTime.Now.Date)
                    {
                        await ExecuteInternalAsync(GitCommitMessageFactory.ScheduleUpdated());
                        _lastUpdateTime = DateTime.Now;
                        _logger.LogInformation("Asset updated. Last executed at= {0}, schedule hour={1}.",
                            _lastUpdateTime, Global.Config.Blog.AssetUpdateAtHour);
                    }
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(AssetHostedService)} is starting.");
            var messages = await _blogService.InitAsync(Global.Config.Blog.CloneAssetsDuringStartup);
            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"{nameof(AssetHostedService)} has started.");
            if (!string.IsNullOrEmpty(messages))
            {
                //TODO: Add direct alert
                _logger.LogWarning($"nameof(AssetHostedService)}} has started, but there are warnings/errors your need to pay attention to.{Environment.NewLine}{messages}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(AssetHostedService)} is stopping.");
            await ExecuteInternalAsync(GitCommitMessageFactory.ServerStopped());
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"{nameof(AssetHostedService)} has stopped.");
        }

        private async Task ExecuteInternalAsync(string message)
        {
            try
            {
                await _blogService.UpdateGitHubAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error popup while updating assets.");
            }
        }
    }
}