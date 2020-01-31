using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Email;
using Laobian.Share.Extension;
using Laobian.Share.Log;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Blog.Alert
{
    public class BlogAlertService : IBlogAlertService
    {
        private readonly IEmailClient _emailClient;
        private readonly ILogger<BlogAlertService> _logger;

        public BlogAlertService(
            IEmailClient emailClient,
            ILogger<BlogAlertService> logger)
        {
            _logger = logger;
            _emailClient = emailClient;
        }

        public async Task AlertReportAsync(string message, Dictionary<string, Stream> logs)
        {
            try
            {
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = $"daily report@{DateTime.Now:yyyyMMdd}",
                    HtmlContent = message
                };

                foreach (var item in logs)
                {
                    emailEntry.Attachments.Add(item.Key, item.Value);
                }

                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "[Need Attention] - Alert report failed.");
            }
        }

        public async Task AlertWarningsAsync(string subject, List<LogEntry> entries)
        {
            try
            {
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = subject,
                    HtmlContent = "<p>Warnings you need to pay attention: </p><ol>"
                };

                foreach (var logEntry in entries)
                {
                    emailEntry.HtmlContent +=
                        $"<li>{logEntry.When.ToDateAndTime()}: {logEntry.Message}<br/><pre><code>{logEntry.Exception}</code></pre></li>";
                }

                emailEntry.HtmlContent += "</ol>";
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "[Need Attention] - Alert warnings failed.");
            }
        }

        public async Task AlertCriticalAsync(string subject, List<LogEntry> entries)
        {
            try
            {
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = subject,
                    HtmlContent = "<p>Critical logs you need to pay attention: </p><ol>"
                };

                foreach (var logEntry in entries)
                {
                    emailEntry.HtmlContent +=
                        $"<li>{logEntry.When.ToDateAndTime()}: {logEntry.Message}<br/><pre><code>{logEntry.Exception}</code></pre></li>";
                }

                emailEntry.HtmlContent += "</ol>";
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,
                    "[Need Attention] - Alert critical failed."); // Email error will be recorded as information
            }
        }

        public async Task AlertErrorsAsync(string subject, List<LogEntry> entries)
        {
            try
            {
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = subject,
                    HtmlContent = "<p>Error logs you need to pay attention: </p><ol>"
                };

                foreach (var logEntry in entries)
                {
                    emailEntry.HtmlContent +=
                        $"<li>{logEntry.When.ToDateAndTime()}: {logEntry.Message}<br/><pre><code>{logEntry.Exception}</code></pre></li>";
                }

                emailEntry.HtmlContent += "</ol>";
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,
                    "[Need Attention] - Alert errors failed."); // Email error will be recorded as information
            }
        }


        public async Task AlertEventAsync(string message, Exception error = null)
        {
            try
            {
                // send alert email out. 
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = "Event Alert",
                    HtmlContent =
                        $"<p>An alert event created, please check if necessary.</p><p><strong>Message body: </strong>{message}</p>"
                };

                if (error != null)
                {
                    emailEntry.HtmlContent +=
                        $"<p><strong>Exception details: </strong></p><p><pre><code>{error}</code></pre></p>";
                }

                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,
                    $"[Need Attention] - Alert event failed. Original Message = {message}, Original Exception = {error}. Current exception = {ex}.");
            }
        }
    }
}