using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Laobian.Share.Log;
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
        private readonly ILogService _logService;

        public PostHostedService(IBlogService blogService, IOptions<AppConfig> appConfig, ILogService logService)
        {
            _logService = logService;
            _blogService = blogService;
            _appConfig = appConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_appConfig.Blog.PostUpdateScheduled)
                    {
                        var chinaTime = DateTime.UtcNow.ToChinaTime();
                        if (chinaTime.Hour == _appConfig.Blog.PostUpdateAtHour && _lastExecuted.Date < chinaTime.Date)
                        {
                            _lastExecuted = chinaTime;
                            await _blogService.UpdateCloudAssetsAsync();
                            await _logService.LogInformation(
                                $"Post hosted service scheduled executed completely, last executed at = {_lastExecuted.ToDateAndTime()}, scheduled at hour = {_appConfig.Blog.PostUpdateAtHour}");
                        }

                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_appConfig.Blog.PostUpdateEverySeconds), stoppingToken);
                        await _blogService.UpdateCloudAssetsAsync();
                        await _logService.LogInformation($"Post hosted service interval executed completely, interval = {_appConfig.Blog.PostUpdateEverySeconds}.");
                    }
                }
                catch (Exception ex)
                {
                    await _logService.LogWarning("Post hosted service execution failed", ex);
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _blogService.UpdateMemoryAssetsAsync(_appConfig.Blog.CloneAssetsDuringStartup);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _blogService.UpdateCloudAssetsAsync();
            await base.StopAsync(cancellationToken);
            _logService.LogInformation("Post hosted service stopped.");
        }
    }
}
