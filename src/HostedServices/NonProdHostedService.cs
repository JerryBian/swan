using Swan.Core.Extension;

namespace Swan.HostedServices
{
    public class NonProdHostedService : BackgroundService
    {
        private readonly ILogger<NonProdHostedService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public NonProdHostedService(ILogger<NonProdHostedService> logger, IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_webHostEnvironment.IsProduction())
            {
                return;
            }

            await Task.Delay(TimeSpan.FromDays(3), stoppingToken).OkForCancel();

            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Non Production site automtically shutdown after 1 hour.");
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
