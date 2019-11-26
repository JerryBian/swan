using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.HostedService
{
    public class LogHostedService : BackgroundService
    {
        private readonly IBlogAlertService _alertService;
        private readonly ILogger<LogHostedService> _logger;
        private DateTime _criticalLogsLastFlushAt;
        private DateTime _errorLogsLastUpdateAt;

        private DateTime _warningLogsLastUpdatedAt;

        public LogHostedService(
            IBlogAlertService alertService,
            ILogger<LogHostedService> logger)
        {
            _alertService = alertService;
            _logger = logger;
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
                    _logger.LogInformation(ex, "[Need Attention] - Log hosted service failed.");
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LogHostedService)} is starting.");
            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"{nameof(LogHostedService)} has started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LogHostedService)} is stopping.");
            await HandleLogsAsync();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"{nameof(LogHostedService)} has stopped.");
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
            if (Global.WarningLogQueue.Count >= Global.Config.Blog.WarningLogsThreshold ||
                DateTime.Now.Hour == Global.Config.Blog.LogFlushAtHour &&
                DateTime.Now.Date > _warningLogsLastUpdatedAt.Date)
            {
                var logs = new List<LogEntry>();
                while (Global.WarningLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                if (logs.Any())
                {
                    // send alert
                    await _alertService.AlertWarningsAsync("WARNINGS - blog", logs);
                }

                _warningLogsLastUpdatedAt = DateTime.Now;
            }
        }

        private async Task HandleErrorLogsAsync()
        {
            if (Global.ErrorLogQueue.Count >= Global.Config.Blog.ErrorLogsThreshold ||
                DateTime.Now.Hour == Global.Config.Blog.LogFlushAtHour &&
                DateTime.Now.Date > _errorLogsLastUpdateAt.Date)
            {
                var logs = new List<LogEntry>();
                while (Global.ErrorLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                if (logs.Any())
                {
                    // send alert
                    await _alertService.AlertErrorsAsync("ERRORS - blog", logs);
                }

                _errorLogsLastUpdateAt = DateTime.Now;
            }
        }

        private async Task HandleCriticalLogsAsync()
        {
            if (Global.CriticalLogQueue.Count >= Global.Config.Blog.CriticalLogsThreshold ||
                DateTime.Now.Hour == Global.Config.Blog.LogFlushAtHour &&
                DateTime.Now.Date > _criticalLogsLastFlushAt.Date)
            {
                var logs = new List<LogEntry>();
                while (Global.CriticalLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                if (logs.Any())
                {
                    // send alert
                    await _alertService.AlertCriticalAsync("CRITICAL - blog", logs);
                }

                _criticalLogsLastFlushAt = DateTime.Now;
            }
        }
    }
}