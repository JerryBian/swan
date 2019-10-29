using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Laobian.Share.Infrastructure.Email;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Blog.HostedService
{
    public class LogHostedService : BackgroundService
    {
        private readonly AppConfig _appConfig;
        private readonly ILogStore _logStore;
        private readonly IEmailClient _emailClient;
        private readonly ILogger<LogHostedService> _logger;

        private DateTime _warningLogsLastUpdatedAt;
        private DateTime _errorLogsLastUpdateAt;

        public LogHostedService(
            ILogStore logStore, 
            ILogger<LogHostedService> logger, 
            IEmailClient emailClient,
            IOptions<AppConfig> appConfig)
        {
            _logStore = logStore;
            _emailClient = emailClient;
            _logger = logger;
            _appConfig = appConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300), stoppingToken);

                try
                {
                    await HandleLogsAsync();
                }
                catch(Exception ex)
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
            if (DateTime.UtcNow - _warningLogsLastUpdatedAt < TimeSpan.FromSeconds(_appConfig.Common.WarningLogsSendInterval))
            {
                return;
            }

            var logs = _logStore.GetLevel(LogLevel.Warning);
            if (!logs.Any())
            {
                return;
            }

            var message = new StringBuilder();
            foreach (var log in logs)
            {
                message.AppendLine($"<p><details>{log}</details></p>");
            }

            message.AppendLine($"<p><small>generated at {DateTime.UtcNow.ToChinaTime().ToDateAndTime()}.</small></p>");
            await _emailClient.SendAsync(
                _appConfig.Blog.ReportSenderName,
                _appConfig.Blog.ReportSenderEmail,
                _appConfig.Common.AdminEnglishName,
                _appConfig.Common.AdminEmail,
                "WARNING LOGS!",
                message.ToString());

            _warningLogsLastUpdatedAt = DateTime.UtcNow;
        }

        private async Task HandleErrorLogsAsync()
        {
            if (DateTime.UtcNow - _errorLogsLastUpdateAt < TimeSpan.FromSeconds(_appConfig.Common.ErrorLogsSendInterval))
            {
                return;
            }

            var logs = _logStore.GetLevel(LogLevel.Error);
            if (!logs.Any())
            {
                return;
            }

            var message = new StringBuilder();
            foreach (var log in logs)
            {
                message.AppendLine($"<p><details>{log}</details></p>");
            }

            message.AppendLine($"<p><small>generated at {DateTime.UtcNow.ToChinaTime().ToDateAndTime()}.</small></p>");
            await _emailClient.SendAsync(
                _appConfig.Blog.ReportSenderName,
                _appConfig.Blog.ReportSenderEmail,
                _appConfig.Common.AdminEnglishName,
                _appConfig.Common.AdminEmail,
                "ERROR LOGS!",
                message.ToString());

            _errorLogsLastUpdateAt = DateTime.UtcNow;
        }

        private async Task HandleCriticalLogsAsync()
        {
            var logs = _logStore.GetLevel(LogLevel.Critical);
            if (!logs.Any())
            {
                return;
            }

            var message = new StringBuilder();
            foreach(var log in logs)
            {
                message.AppendLine($"<p><details>{log}</details></p>");
            }

            message.AppendLine($"<p><small>generated at {DateTime.UtcNow.ToChinaTime().ToDateAndTime()}.</small></p>");
            await _emailClient.SendAsync(
                _appConfig.Blog.ReportSenderName,
                _appConfig.Blog.ReportSenderEmail,
                _appConfig.Common.AdminEnglishName,
                _appConfig.Common.AdminEmail,
                "CRITICAL LOGS!",
                message.ToString());
        }
    }
}
