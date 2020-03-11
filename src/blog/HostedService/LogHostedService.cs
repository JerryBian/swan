using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Extension;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.HostedService
{
    public class LogHostedService : BackgroundService
    {
        private readonly IBlogAlertService _alertService;
        private readonly ILogger<LogHostedService> _logger;
        private DateTime _lastReportGeneratedAt;

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
                    await CheckLogsAsync();

                    if (DateTime.Now.Date > _lastReportGeneratedAt.Date &&
                        DateTime.Now.Hour == Global.Config.Blog.LogFlushAtHour)
                    {
                        await GenerateLogReportAsync();
                        _lastReportGeneratedAt = DateTime.Now;
                        _logger.LogInformation("Report generated completely.");
                    }

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
            await GenerateLogReportAsync();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"{nameof(LogHostedService)} has stopped.");
        }

        private async Task GenerateLogReportAsync()
        {
            var logs = new Dictionary<string, Stream>();
            var warnStream = GetWarnStream();
            logs.Add("warn-log.txt", warnStream);
            var errorStream = GetErrorStream();
            logs.Add("error-log.txt", errorStream);

            using (warnStream)
            using (errorStream)
            {
                await _alertService.AlertReportAsync("<p>Please check attached daily reports of logs.</p>", logs);
            }
        }

        private static MemoryStream GetErrorStream()
        {
            var errorLogs = new List<LogEntry>();
            while (Global.ErrorLogQueue.TryDequeue(out var errorLog))
            {
                errorLogs.Add(errorLog);
            }

            var errorMessage = "Good! There is no error logs today!";
            if (errorLogs.Any())
            {
                errorMessage = "Please check error logs:";
                foreach (var logEntry in errorLogs)
                {
                    errorMessage += Environment.NewLine + Environment.NewLine +
                                    $"{logEntry.When.ToDateAndTime()}\t{logEntry.Message}\t{logEntry.Exception}";
                }
            }

            var errorStream = new MemoryStream(Encoding.UTF8.GetBytes(errorMessage));
            return errorStream;
        }

        private static MemoryStream GetWarnStream()
        {
            var warnLogs = new List<LogEntry>();
            while (Global.WarningLogQueue.TryDequeue(out var warnLog))
            {
                warnLogs.Add(warnLog);
            }

            var warnMessage = "Good! There is no warning logs today!";
            if (warnLogs.Any())
            {
                warnMessage = "Please check warning logs:";
                foreach (var logEntry in warnLogs)
                {
                    warnMessage += Environment.NewLine + Environment.NewLine +
                                   $"{logEntry.When.ToDateAndTime()}\t{logEntry.Message}\t{logEntry.Exception}";
                }
            }

            var warnStream = new MemoryStream(Encoding.UTF8.GetBytes(warnMessage));
            return warnStream;
        }

        private async Task CheckLogsAsync()
        {
            var tasks = new List<Task>
            {
                CheckCriticalLogsAsync(),
                CheckErrorLogsAsync(),
                CheckWarningLogsAsync()
            };

            await Task.WhenAll(tasks);
        }

        private async Task CheckWarningLogsAsync()
        {
            if (Global.WarningLogQueue.Count >= Global.Config.Blog.WarningLogsThreshold)
            {
                var logs = new List<LogEntry>();
                while (Global.WarningLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                if (logs.Any())
                {
                    // send alert
                    await _alertService.AlertWarningsAsync("Many Warnings in blog", logs);
                }
            }
        }

        private async Task CheckErrorLogsAsync()
        {
            if (Global.ErrorLogQueue.Count >= Global.Config.Blog.ErrorLogsThreshold)
            {
                var logs = new List<LogEntry>();
                while (Global.ErrorLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                if (logs.Any())
                {
                    // send alert
                    await _alertService.AlertErrorsAsync("Many errors in blog", logs);
                }
            }
        }

        private async Task CheckCriticalLogsAsync()
        {
            if (Global.CriticalLogQueue.Count >= Global.Config.Blog.CriticalLogsThreshold)
            {
                var logs = new List<LogEntry>();
                while (Global.CriticalLogQueue.TryDequeue(out var log))
                {
                    logs.Add(log);
                }

                if (logs.Any())
                {
                    // send alert
                    await _alertService.AlertCriticalAsync("Critical log in blog", logs);
                }
            }
        }
    }
}