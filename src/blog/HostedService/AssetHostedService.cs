using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.HostedService
{
    public class AssetHostedService : BackgroundService
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public AssetHostedService(IBlogService blogService, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _blogService.ReloadAssetsAsync(_appConfig.Blog.CloneAssetsDuringStartup,
                _appConfig.Blog.CloneAssetsDuringStartup);
            await base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
