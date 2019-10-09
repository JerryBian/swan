using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HostedService
{
    public class PostHostedService : BackgroundService
    {
        private DateTime _lastExecuted;
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public PostHostedService(IBlogService blogService, IOptions<AppConfig> appConfig)
        {
            _blogService = blogService;
            _appConfig = appConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var chinaTime = DateTime.UtcNow.ToChinaTime();
                    if (chinaTime.Hour == 4 && _lastExecuted.Date < chinaTime.Date)
                    {
                        _lastExecuted = chinaTime;
                        await _blogService.UpdateCloudAssetsAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch { }
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
        }
    }
}
