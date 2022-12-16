using Swan.Lib.Extension;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken).OkForCancel();

                try
                {
                    _logService.Cleanup();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cleanup log files failed.");
                }
            }
        }
    }
}
