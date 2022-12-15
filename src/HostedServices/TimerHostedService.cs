namespace Swan.HostedServices
{
    public class TimerHostedService : BackgroundService
    {
        private DateTime _startedAt;
        private readonly ILogger<TimerHostedService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public TimerHostedService(ILogger<TimerHostedService> logger, IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _startedAt = DateTime.Now;
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_webHostEnvironment.IsStaging() && DateTime.Now - _startedAt > TimeSpan.FromDays(1))
                {
                    _logger.LogInformation($"Staging site automtically shutdown after 1 hour.");
                    _hostApplicationLifetime.StopApplication();
                    return;
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
