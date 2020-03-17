using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Email;
using Laobian.Share.Extension;
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

        public async Task AlertReportAsync(List<BlogAlertEntry> warnAlerts, List<BlogAlertEntry> errorAlerts)
        {
            try
            {
                var template = await File.ReadAllTextAsync("/Blog/Alert/ReportTemplate.txt");
                if (string.IsNullOrEmpty(template))
                {
                    await AlertEventAsync("Either the report template is empty or failed to find.");
                    return;
                }

                var errorSb = new StringBuilder();
                foreach (var item in errorAlerts)
                {
                    errorSb.AppendLine($"<section><h4>{item.Message}</h4>");
                    errorSb.AppendLine($"<details><summary>Exception StackTrace</summary><pre>{item.Exception}</pre></details>");
                    errorSb.AppendLine("<table><tbody>");
                    errorSb.AppendLine($"<tr><td>IP</td><td>{item.Ip}</td></tr>");
                    errorSb.AppendLine($"<tr><td>Request URL</td><td>{item.RequestUrl}</td></tr>");
                    errorSb.AppendLine($"<tr><td>User Agent</td><td>{item.UserAgent}</td></tr>");
                    errorSb.AppendLine("</tbody></table></section>");
                }

                var warnSb = new StringBuilder();
                foreach (var item in warnAlerts)
                {
                    warnSb.AppendLine($"<section><h4>{item.Message}</h4>");
                    warnSb.AppendLine("<table><tbody>");
                    warnSb.AppendLine($"<tr><td>IP</td><td>{item.Ip}</td></tr>");
                    warnSb.AppendLine($"<tr><td>Request URL</td><td>{item.RequestUrl}</td></tr>");
                    warnSb.AppendLine($"<tr><td>User Agent</td><td>{item.UserAgent}</td></tr>");
                    warnSb.AppendLine("</tbody></table></section>");
                }

                template = template
                    .Replace("##Error-Count##", errorAlerts.Count.ToString())
                    .Replace("##Warn-Count##", warnAlerts.Count.ToString())
                    .Replace("##Error-Sections##", errorSb.ToString())
                    .Replace("##Warn-Sections##", warnSb.ToString())
                    .Replace("##Report-Time##", DateTime.Now.ToDateAndTime());
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = $"Report@{DateTime.Now:yyyyMMdd}",
                    HtmlContent = $"<small>{errorAlerts.Count.Human()} Errors, {warnAlerts.Count.Human()} Warnings.</small>"
                };

                await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(template));
                emailEntry.Attachments.Add($"report@{DateTime.Now:yyyyMMdd}.html", ms);
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "[Need Attention] - Alert report failed.");
            }
        }
    }
}