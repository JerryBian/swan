using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HostedService
{
    public class AssetHostedService : BackgroundService
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<AssetHostedService> _logger;

        private DateTime _lastUpdateTime;

        public AssetHostedService(IBlogService blogService, IOptions<AppConfig> appConfig, IWebHostEnvironment environment, ILogger<AssetHostedService> logger)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
            _blogService = blogService;
            _environment = environment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_environment.IsDevelopment())
                {
                    if (DateTime.Now - _lastUpdateTime < TimeSpan.FromMinutes(1))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    }
                    else
                    {
                        await _blogService.UpdateRemoteAssetsAsync();
                        _lastUpdateTime = DateTime.Now;
                        _logger.LogInformation("Asset updated. Last executed at= {0}.", _lastUpdateTime);
                    }
                }
                else
                {
                    if (DateTime.Now.Hour == _appConfig.Blog.PostUpdateAtHour &&
                        _lastUpdateTime.Date < DateTime.Now.Date)
                    {
                        await _blogService.UpdateRemoteAssetsAsync();
                        _lastUpdateTime = DateTime.Now;
                        _logger.LogInformation("Asset updated. Last executed at= {0}, schedule hour={1}.", _lastUpdateTime, _appConfig.Blog.PostUpdateAtHour);
                    }
                }

            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _blogService.ReloadLocalAssetsAsync(_appConfig.Blog.CloneAssetsDuringStartup,
                _appConfig.Blog.CloneAssetsDuringStartup);
            _logger.LogInformation("AssetHostedService start completed.");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _blogService.UpdateRemoteAssetsAsync();
            _logger.LogInformation("AssetHostedService stop completed.");
            await base.StopAsync(cancellationToken);
        }
    }
}
