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
using Laobian.Share.Helper;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;
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
                    Subject = "new event happened!",
                    HtmlContent = $"<div>{message}</div>"
                };

                if (error != null)
                {
                    emailEntry.HtmlContent +=
                        $"<p><strong>Exception details: </strong></p><p><pre><code>{error}</code></pre></p>";
                }

                emailEntry.HtmlContent += GetServerStates();
                await _emailClient.SendAsync(emailEntry);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex,
                    $"[Need Attention] - Alert event failed. Original Message = {message}, Original Exception = {error}. Current exception = {ex}.");
            }
        }

        public async Task AlertLogsAsync(List<LogEntry> logs)
        {
            if (!logs.Any())
            {
                _logger.LogInformation("No need to generate log report as there is no warnings/errors yesterday.");
                if (!Global.Environment.IsDevelopment())
                {
                    await AlertEventAsync("No need to generate log report as there is no warnings/errors yesterday.");
                }
                
                return;
            }

            try
            {
                _logger.LogInformation("Start to generate log report.");
                var binDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var fullPath = Path.Combine(binDir, "./Blog/Alert/ReportTemplate.txt");
                var template = await File.ReadAllTextAsync(fullPath);
                if (string.IsNullOrEmpty(template))
                {
                    await AlertEventAsync("<p>Either the report template is empty or failed to find.</p>");
                    return;
                }

                var errorSb = new StringBuilder();
                var errors = logs.Where(l => l.Level == LogLevel.Error).OrderByDescending(l => l.When).ToList();
                foreach (var item in errors)
                {
                    errorSb.AppendLine($"<section><h4>[{item.When.ToDateAndTime()}] {item.Message}</h4>");
                    if (item.Exception != null)
                    {
                        errorSb.AppendLine($"<div><pre>{item.Exception}</pre></div>");
                    }

                    errorSb.AppendLine("</section>");
                }

                var warnSb = new StringBuilder();
                var warns = logs.Where(l => l.Level == LogLevel.Warning).OrderByDescending(l => l.When).ToList();
                foreach (var item in warns)
                {
                    warnSb.AppendLine($"<section><h4>[{item.When.ToDateAndTime()}] {item.Message}</h4>");
                    if (item.Exception != null)
                    {
                        warnSb.AppendLine($"<div><pre>{item.Exception}</pre></div>");
                    }

                    warnSb.AppendLine("</section>");
                }

                template = template
                    .Replace("##Error-Count##", errors.Count.ToString())
                    .Replace("##Warn-Count##", warns.Count.ToString())
                    .Replace("##Error-Sections##", errorSb.ToString())
                    .Replace("##Warn-Sections##", warnSb.ToString())
                    .Replace("##Report-Time##", DateTime.Now.ToDateAndTime())
                    .Replace("##Server-State##", GetServerStates());
                var emailEntry = new EmailEntry(Global.Config.Common.AdminEnglishName, Global.Config.Common.AdminEmail)
                {
                    FromName = Global.Config.Common.AlertSenderName,
                    FromAddress = Global.Config.Common.AlertSenderEmail,
                    Subject = $"Report@{DateTime.Now:yyyyMMdd}",
                    HtmlContent =
                        $"<p>There are <strong>{HumanHelper.DisplayInt(errors.Count)}</strong> Errors, <strong>{HumanHelper.DisplayInt(warns.Count)}</strong> Warnings for yesterday, please check more details in attachment.</p>"
                };

                await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(template));
                emailEntry.Attachments.Add($"report@{DateTime.Now:yyyyMMdd}.html", ms);
                await _emailClient.SendAsync(emailEntry);
                _logger.LogInformation("Finished generate log report.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate log report failed.");
            }
        }

        private string GetServerStates()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-size: 80%;background-color:e8e4e1;margin: 1rem 0;padding:1%;'>");
            using (var process = Process.GetCurrentProcess())
            {
                sb.AppendLine(
                    $"<p><strong>Boot:</strong> {Global.StartTime.ToDateAndTime()}({Global.RunningInterval}).</p>");
                sb.AppendLine(
                    $"<p><strong>Machine:</strong> {Environment.MachineName} / {TimeZoneInfo.Local.StandardName} / {Environment.UserName}.</p>");
                sb.AppendLine(
                    $"<p><strong>Version:</strong> {Environment.OSVersion} / {Global.RuntimeVersion} / {Global.AppVersion}.</p>");
                sb.AppendLine(
                    $"<p><strong>Resource:</strong> {Environment.ProcessorCount}(processors) / {HumanHelper.DisplayBytes(process.WorkingSet64)}(memory) / {HumanHelper.DisplayTimeSpan(process.TotalProcessorTime)}(CPU).</p>");
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }
    }
}