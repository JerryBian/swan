using Swan.Core.Extension;

namespace Swan.Web.HostedServices
{
    public class MonitorHostedService : BackgroundService
    {
        private readonly DateTime _startAt;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<MonitorHostedService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public MonitorHostedService(
            IHostEnvironment hostEnvironment,
            ILogger<MonitorHostedService> logger,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _startAt = DateTime.Now;
            _hostEnvironment = hostEnvironment;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken).OkForCancel();

                if (!stoppingToken.IsCancellationRequested)
                {
                    if (DateTime.Now - _startAt > TimeSpan.FromDays(3) && !_hostEnvironment.IsProduction())
                    {
                        _hostApplicationLifetime.StopApplication();
                        return;
                    }
                }

            }
        }
    }
}
