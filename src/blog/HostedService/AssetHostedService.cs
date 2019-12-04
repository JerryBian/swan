using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.HostedService
{
    public class AssetHostedService : BackgroundService
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<AssetHostedService> _logger;

        private DateTime _lastUpdateTime;

        public static event EventHandler<BlogAssetChange> BlogAssetChangeEvent;

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
                    await _blogService.UpdateRemoteAssetsAsync();
                    _lastUpdateTime = DateTime.Now;
                    _logger.LogInformation("Asset updated. Last executed at= {0}.", _lastUpdateTime);
                }
                else
                {
                    if (DateTime.Now.Hour == Global.Config.Blog.AssetUpdateAtHour &&
                        _lastUpdateTime.Date < DateTime.Now.Date)
                    {
                        await _blogService.UpdateRemoteAssetsAsync();
                        _lastUpdateTime = DateTime.Now;
                        _logger.LogInformation("Asset updated. Last executed at= {0}, schedule hour={1}.",
                            _lastUpdateTime, Global.Config.Blog.AssetUpdateAtHour);
                    }
                }

                var posts = _blogService.GetPosts();
                var nextRefreshAt = posts
                    .Where(p => p.GetRawPublishTime() != null && p.GetRawPublishTime() > DateTime.Now)
                    .Min(p => p.GetRawPublishTime());
                if (nextRefreshAt != null && nextRefreshAt != default(DateTime))
                {
                    BlogAssetChangeEvent?.Invoke(this, new BlogAssetChange { NextRefreshAt = nextRefreshAt.Value });
                }

                _logger.LogDebug("Next refresh at {0}.", nextRefreshAt);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(AssetHostedService)} is starting.");
            await _blogService.ReloadLocalAssetsAsync(Global.Config.Blog.CloneAssetsDuringStartup,
                Global.Config.Blog.CloneAssetsDuringStartup);

            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"{nameof(AssetHostedService)} has started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(AssetHostedService)} is stopping.");
            await _blogService.UpdateRemoteAssetsAsync();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"{nameof(AssetHostedService)} has stopped.");
        }
    }
}