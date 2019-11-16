using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Config;
using Laobian.Share.Email;
using Laobian.Share.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Blog.Alert
{
    public class BlogAlertService : IBlogAlertService
    {
        private readonly AppConfig _appConfig;
        private readonly IEmailClient _emailClient;
        private readonly ILogger<BlogAlertService> _logger;

        public BlogAlertService(
            IOptions<AppConfig> appConfig,
            IEmailClient emailClient,
            ILogger<BlogAlertService> logger)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
            _emailClient = emailClient;
        }

        public async Task AlertWarningsAsync(string subject, List<LogEntry> entries)
        {
            try
            {
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = subject,
                    HtmlContent = $"<p>Warnings you need to pay attention: </p><ol>"
                };

                foreach (var logEntry in entries)
                {
                    emailEntry.HtmlContent +=
                        $"<li>{logEntry.Message}<br/><pre><code>{logEntry.Exception}</code></pre></li>";
                }

                emailEntry.HtmlContent += "</ol>";
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Alert warnings failed.");
            }
        }
        public async Task AlertCriticalAsync(string subject, List<LogEntry> entries)
        {
            try
            {
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = subject,
                    HtmlContent = $"<p>Critical logs you need to pay attention: </p><ol>"
                };

                foreach (var logEntry in entries)
                {
                    emailEntry.HtmlContent +=
                        $"<li>{logEntry.Message}<br/><pre><code>{logEntry.Exception}</code></pre></li>";
                }

                emailEntry.HtmlContent += "</ol>";
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Alert critical failed."); // Email error will be recorded as information
            }
        }

        public async Task AlertErrorsAsync(string subject, List<LogEntry> entries)
        {
            try
            {
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = subject,
                    HtmlContent = $"<p>Error logs you need to pay attention: </p><ol>"
                };

                foreach (var logEntry in entries)
                {
                    emailEntry.HtmlContent +=
                        $"<li>{logEntry.Message}<br/><pre><code>{logEntry.Exception}</code></pre></li>";
                }

                emailEntry.HtmlContent += "</ol>";
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Alert errors failed."); // Email error will be recorded as information
            }
        }


        public async Task AlertEventAsync(string message, Exception error = null)
        {
            try
            {
                // send alert email out. 
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = "Event Alert",
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
                _logger.LogInformation(ex, $"Alert accident failed. Original Message = {message}, Original Exception = {error}. Current exception = {ex}.");
            }
        }

        public async Task AlertAssetReloadResultAsync(string subject, string warning, string error, List<string> addedPosts = null, List<string> modifiedPosts = null)
        {
            try
            {
                // send alert email out. 
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = subject,
                    HtmlContent = $"<p>Reload assets finished, please check.</p>"
                };

                if (addedPosts != null && addedPosts.Any())
                {
                    emailEntry.HtmlContent += "<p><strong>Added posts: </strong></p><ul>";
                    foreach (var addedPost in addedPosts)
                    {
                        emailEntry.HtmlContent += $"<li>{addedPost}</li>";
                    }

                    emailEntry.HtmlContent += "</ul>";
                }

                if (modifiedPosts != null && modifiedPosts.Any())
                {
                    emailEntry.HtmlContent += "<p><strong>Modified posts: </strong></p><ul>";
                    foreach (var modifiedPost in modifiedPosts)
                    {
                        emailEntry.HtmlContent += $"<li>{modifiedPost}</li>";
                    }

                    emailEntry.HtmlContent += "</ul>";
                }

                if (!string.IsNullOrEmpty(warning))
                {
                    emailEntry.HtmlContent += $"<p><strong>Warnings: </strong></p><p><pre><code>{warning}</code></pre></p>";
                }

                if (!string.IsNullOrEmpty(error))
                {
                    emailEntry.HtmlContent += $"<p><strong>Errors: </strong></p><p><pre><code>{error}</code></pre></p>";
                }

                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Alert assets reloading failed, subject = {subject}, warning = {warning}, error = {error}.", ex,
                    true);
            }
        }
    }
}
