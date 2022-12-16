using Swan.Lib.Extension;

namespace Swan.HostedServices
{
    public class AutoShutdownHostedService : BackgroundService
    {
        private readonly ILogger<AutoShutdownHostedService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public AutoShutdownHostedService(ILogger<AutoShutdownHostedService> logger, IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(_webHostEnvironment.IsProduction())
            {
                return;
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken).OkForCancel();

            if(!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Non Production site automtically shutdown after 1 hour.");
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
