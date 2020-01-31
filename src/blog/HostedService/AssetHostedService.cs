using System;
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
                    await ExecuteInternalAsync();
                    _lastUpdateTime = DateTime.Now;
                    _logger.LogInformation("Asset updated. Last executed at= {0}.", _lastUpdateTime);
                }
                else
                {
                    if (DateTime.Now.Hour == Global.Config.Blog.AssetUpdateAtHour &&
                        _lastUpdateTime.Date < DateTime.Now.Date)
                    {
                        await ExecuteInternalAsync();
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
<<<<<<< HEAD
            await _blogService.InitAsync(Global.Config.Blog.CloneAssetsDuringStartup);
=======
            await _blogService.ReloadLocalAssetsAsync(Global.Config.Blog.CloneAssetsDuringStartup,
                Global.Config.Blog.CloneAssetsDuringStartup);
            RefreshPostAccessCount();
>>>>>>> master
            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"{nameof(AssetHostedService)} has started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(AssetHostedService)} is stopping.");
            await _blogService.UpdateGitHubAsync();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"{nameof(AssetHostedService)} has stopped.");
        }

        private async Task ExecuteInternalAsync()
        {
<<<<<<< HEAD
            try
            {
                await _blogService.UpdateGitHubAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error popup while updating assets.");
            }
=======
            await _blogService.UpdateRemoteAssetsAsync();
            RefreshPostAccessCount();
        }

        private void RefreshPostAccessCount()
        {
            var visitTotal = 0;
            foreach (var blogPost in _blogService.GetPosts(false))
            {
                blogPost.AccessCount = _blogService.GetPostAccess().Get(blogPost.GitPath);
                visitTotal += blogPost.AccessCount;
            }

            BlogState.PostsVisitsTotal = visitTotal;
>>>>>>> master
        }
    }
}