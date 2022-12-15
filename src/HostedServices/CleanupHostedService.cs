using Swan.Lib.Service;

namespace Swan.HostedServices
{
    public class CleanupHostedService : BackgroundService
    {
        private readonly ILogService _logService;
        private readonly ILogger<CleanupHostedService> _logger;

        public CleanupHostedService(ILogger<CleanupHostedService> logger, ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime lastExecuteAt = DateTime.Now;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now - lastExecuteAt < TimeSpan.FromMinutes(1))
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                if (DateTime.Now.Hour == 1 && DateTime.Now.Minute == 0)
                {
                    try
                    {
                        _logService.Cleanup();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Cleanup log files failed.");
                    }
                    finally
                    {
                        lastExecuteAt = DateTime.Now;
                    }
                }
            }
        }
    }
}
