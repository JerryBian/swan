using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HostedService
{
    public class PostHostedService : BackgroundService
    {
        private DateTime _lastExecuted;
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;
        private readonly ILogger<PostHostedService> _logger;

        public PostHostedService(IBlogService blogService, IOptions<AppConfig> appConfig, ILogger<PostHostedService> logger)
        {
            _logger = logger;
            _blogService = blogService;
            _appConfig = appConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_appConfig.PostUpdateScheduled)
                    {
                        var chinaTime = DateTime.UtcNow.ToChinaTime();
                        if (chinaTime.Hour == _appConfig.PostUpdateAtHour && _lastExecuted.Date < chinaTime.Date)
                        {
                            _lastExecuted = chinaTime;
                            await _blogService.UpdateCloudAssetsAsync();
                            _logger.LogInformation(
                                "Post hosted service scheduled executed completely, last executed at = {LastExecutedAt}, scheduled at hour = {Hour}", 
                                _lastExecuted.ToDateAndTime(),
                                _appConfig.PostUpdateAtHour);
                        }

                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_appConfig.PostUpdateEverySeconds), stoppingToken);
                        await _blogService.UpdateCloudAssetsAsync();
                        _logger.LogInformation("Post hosted service interval executed completely, interval = {Interval}.", _appConfig.PostUpdateEverySeconds);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Post hosted service execution failed");
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _blogService.UpdateMemoryAssetsAsync(_appConfig.CloneAssetsDuringStartup);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _blogService.UpdateCloudAssetsAsync();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Post hosted service stopped.");
        }
    }
}
