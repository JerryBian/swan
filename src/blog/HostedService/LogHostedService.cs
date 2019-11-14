using Laobian.Share.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Log;

namespace Laobian.Blog.HostedService
{
    public class LogHostedService : BackgroundService
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogAlertService _alertService;
        private readonly ILogger<LogHostedService> _logger;

        private DateTime _warningLogsLastUpdatedAt;
        private DateTime _errorLogsLastUpdateAt;
        private DateTime _criticalLogsLastFlushAt;

        public LogHostedService(
            IBlogAlertService alertService,
            ILogger<LogHostedService> logger,
            IOptions<AppConfig> appConfig)
        {
            _alertService = alertService;
            _logger = logger;
            _appConfig = appConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                try
                {
                    await HandleLogsAsync();
                    _logger.LogInformation("Logs flushed completely.");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Log hosted service failed.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await HandleLogsAsync();
            await base.StopAsync(cancellationToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        private async Task HandleLogsAsync()
        {
            var tasks = new List<Task>
            {
                HandleCriticalLogsAsync(),
                HandleErrorLogsAsync(),
                HandleWarningLogsAsync()
            };

            await Task.WhenAll(tasks);
        }

        private async Task HandleWarningLogsAsync()
        {
            if (MemoryStore.WarningLogQueue.Count >= _appConfig.Blog.WarningLogsThreshold ||
                DateTime.Now.Hour == _appConfig.Blog.LogFlushAtHour &&
                DateTime.Now.Date > _warningLogsLastUpdatedAt.Date)
            {
                var logs = new List<LogEntry>();
                while (MemoryStore.WarningLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                // send alert

                _warningLogsLastUpdatedAt = DateTime.Now;
            }
        }

        private async Task HandleErrorLogsAsync()
        {
            if (MemoryStore.ErrorLogQueue.Count >= _appConfig.Blog.ErrorLogsThreshold ||
                DateTime.Now.Hour == _appConfig.Blog.LogFlushAtHour &&
                DateTime.Now.Date > _errorLogsLastUpdateAt.Date)
            {
                var logs = new List<LogEntry>();
                while (MemoryStore.ErrorLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                // send alert

                _errorLogsLastUpdateAt = DateTime.Now;
            }
        }

        private async Task HandleCriticalLogsAsync()
        {
            if (MemoryStore.CriticalLogQueue.Count >= _appConfig.Blog.CriticalLogsThreshold ||
                DateTime.Now.Hour == _appConfig.Blog.LogFlushAtHour &&
                DateTime.Now.Date > _criticalLogsLastFlushAt.Date)
            {
                var logs = new List<LogEntry>();
                while (MemoryStore.CriticalLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                // send alert

                _criticalLogsLastFlushAt = DateTime.Now;
            }
        }
    }
}
