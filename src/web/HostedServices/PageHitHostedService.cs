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
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken).OkForCancel();
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
                var rawPages = await _swanStore.GetPageHitsAsync();
                if (rawPages.Count == 0) return;

                // Normalize paths and aggregate hit counts locally
                var pathHits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var rawPage in rawPages)
                {
                    var path = rawPage;
                    if (!string.IsNullOrEmpty(path) && path.Length > 1 && path.EndsWith('/'))
                    {
                        path = path.TrimEnd('/');
                    }
                    if (pathHits.ContainsKey(path))
                    {
                        pathHits[path]++;
                    }
                    else
                    {
                        pathHits[path] = 1;
                    }
                }

                // Single batched read+write under one lock
                await _swanService.BulkUpdatePageHitsAsync(pathHits);

                stopwatch.Stop();
                _logger.LogInformation($"Flush page stats({rawPages.Count} raw hits, {pathHits.Count} unique pages) completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to flush page stats.");
            }
        }
    }
}
