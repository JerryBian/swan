using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Service;
using Swan.Core.Store;
using System.Diagnostics;

namespace Swan.Web.HostedServices
{
    public class PageHitHostedService : BackgroundService
    {
        private readonly ISwanStore _swanStore;
        private readonly ISwanService _swanService;
        private readonly ILogger<PageHitHostedService> _logger;

        public PageHitHostedService(ISwanStore swanStore, ISwanService swanService, ILogger<PageHitHostedService> logger)
        {
            _swanStore = swanStore;
            _swanService = swanService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken).OkForCancel();
                await FlushPageHitsAsync();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await FlushPageHitsAsync();
            await base.StopAsync(cancellationToken);
        }

        private async Task FlushPageHitsAsync()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var pages = await _swanStore.GetPageHitsAsync();
                foreach (var page in pages)
                {
                    var stat = await _swanService.FindFirstOrDefaultAsync<SwanPage>(true, x => StringHelper.EqualsIgoreCase(x.Path, page));
                    if (stat == null)
                    {
                        await _swanService.AddAsync<SwanPage>(new SwanPage { Path = page, Hit = 1 });
                    }
                    else
                    {
                        stat.Hit += 1;
                        await _swanService.UpdateAsync(stat);
                    }
                }

                stopwatch.Stop();
                _logger.LogInformation($"Flush page stats({pages.Count} records) completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to flush page stats.");
            }
        }
    }
}
