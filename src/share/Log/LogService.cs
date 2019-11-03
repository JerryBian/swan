using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Config;
using Laobian.Share.Infrastructure.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Log
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly AppConfig _appConfig;
        private readonly IEmailClient _emailClient;

        public LogService(ILogger<LogService> logger, IEmailClient emailClient, IOptions<AppConfig> appConfig)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
            _emailClient = emailClient;
        }

        public async Task LogInformation(string message, bool alert = false)
        {
            _logger.LogInformation(message);

            if (alert)
            {
                await AlertAsync(message);
            }
        }

        public async Task LogWarning(string message, Exception ex = null, bool alert = false)
        {
            _logger.LogWarning(ex, message);
            AddToMemoryStore(message, ex, LogLevel.Warning);

            if (alert)
            {
                await AlertAsync(message, ex);
            }
        }

        public async Task LogError(string message, Exception ex = null, bool alert = false)
        {
            _logger.LogError(ex, message);
            AddToMemoryStore(message, ex, LogLevel.Error);

            if (alert)
            {
                await AlertAsync(message, ex);
            }
        }

        private void AddToMemoryStore(string message, Exception ex, LogLevel level)
        {
            MemoryStore.LogQueue.Enqueue(new LogEntry
            {
                Exception = ex,
                Level = LogLevel.Warning,
                Message = message,
                When = DateTimeOffset.Now
            });
        }

        private async Task AlertAsync(string message, Exception error = null)
        {
            try
            {
                // send alert email out. 
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = "Alert Event",
                    HtmlContent = $"<p>An alert event created, please check.</p><p><strong>Message body: </strong>{message}</p>"
                };

                if (error != null)
                {
                    emailEntry.HtmlContent += $"<p><strong>Exception details: </strong></p><p><pre><code>{error}</code></pre></p>";
                }

                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                await LogError($"Alert failed. Message = {message}, Exception = {error}.", ex); // we cannot alert here!
            }
        }
    }
}
