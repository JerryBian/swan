using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
                    Subject = "New event happened",
                    HtmlContent = $"<p>{message}</p>"
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
            if (!warnAlerts.Any() && !errorAlerts.Any())
            {
                return;
            }

            try
            {
                var binDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var fullPath = Path.Combine(binDir, "./Blog/Alert/ReportTemplate.txt");
                var template = await File.ReadAllTextAsync(fullPath);
                if (string.IsNullOrEmpty(template))
                {
                    await AlertEventAsync("Either the report template is empty or failed to find.");
                    return;
                }

                var errorSb = new StringBuilder();
                foreach (var item in errorAlerts)
                {
                    errorSb.AppendLine($"<section><h4>({item.When.ToDateAndTime()}) {item.Message}</h4>");
                    if (!string.IsNullOrEmpty(item.RequestUrl))
                    {
                        errorSb.AppendLine($"<details><summary>Exception StackTrace</summary><pre>{item.Exception}</pre></details>");
                        errorSb.AppendLine("<table><tbody>");
                        errorSb.AppendLine($"<tr><td>IP</td><td>{item.Ip}</td></tr>");
                        errorSb.AppendLine($"<tr><td>Request URL</td><td>{item.RequestUrl}</td></tr>");
                        errorSb.AppendLine($"<tr><td>User Agent</td><td>{item.UserAgent}</td></tr>");
                        errorSb.AppendLine("</tbody></table>");
                    }

                    errorSb.AppendLine("</section>");
                }

                var warnSb = new StringBuilder();
                foreach (var item in warnAlerts)
                {
                    warnSb.AppendLine($"<section><h4>({item.When.ToDateAndTime()}) {item.Message}</h4>");
                    if (!string.IsNullOrEmpty(item.RequestUrl))
                    {
                        warnSb.AppendLine("<table><tbody>");
                        warnSb.AppendLine($"<tr><td>IP</td><td>{item.Ip}</td></tr>");
                        warnSb.AppendLine($"<tr><td>Request URL</td><td>{item.RequestUrl}</td></tr>");
                        warnSb.AppendLine($"<tr><td>User Agent</td><td>{item.UserAgent}</td></tr>");
                        warnSb.AppendLine("</tbody></table>");
                    }

                    warnSb.AppendLine("</section>");
                }

                template = template
                    .Replace("##Error-Count##", errorAlerts.Count.ToString())
                    .Replace("##Warn-Count##", warnAlerts.Count.ToString())
                    .Replace("##Error-Sections##", errorSb.ToString())
                    .Replace("##Warn-Sections##", warnSb.ToString())
                    .Replace("##Report-Time##", DateTime.Now.ToDateAndTime())
                    .Replace("##Server-State##", GetServerStates());
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = $"Report@{DateTime.Now:yyyyMMdd}",
                    HtmlContent = $"<p>{errorAlerts.Count.Human()} Errors, {warnAlerts.Count.Human()} Warnings.</p>"
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

        private string GetServerStates()
        {
            var sb = new StringBuilder();
            using (var process = Process.GetCurrentProcess())
            {
                sb.AppendLine($"<li>Start at {Global.StartTime.ToDateAndTime()}, it has been running for {Global.RunningInterval}.</li>");
                sb.AppendLine($"<li>Process Id: {process.Id}, process name: {process.ProcessName}.</li>");
                sb.AppendLine($"<li>Host name: {Environment.MachineName}, current user: {Environment.UserName}.</li>");
                sb.AppendLine($"<li>OS version: {Environment.OSVersion}, .NET version: {Global.RuntimeVersion}, app version: {Global.AppVersion}</li>");
                sb.AppendLine($"<li>Processor count: {Environment.ProcessorCount}, Allocated memory: {process.WorkingSet64.HumanByte()}, CPU time: {process.TotalProcessorTime.Human()}.</li>");
            }

            return sb.ToString();
        }
    }
}