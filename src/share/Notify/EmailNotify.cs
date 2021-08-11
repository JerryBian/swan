using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ByteSizeLib;
using Laobian.Share.Extension;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Laobian.Share.Notify
{
    public class EmailNotify : IEmailNotify
    {
        private readonly ILogger<EmailNotify> _logger;

        public EmailNotify(ILogger<EmailNotify> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendAsync(NotifyMessage message)
        {
            if (string.IsNullOrEmpty(message.SendGridApiKey))
            {
                Console.WriteLine($"No SendGrid Api Key provided. ==> {JsonUtil.Serialize(message)}");
                return false;
            }

            var client = new SendGridClient(message.SendGridApiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress($"{message.Site.ToString().ToLowerInvariant()}@laobian.me",
                    $"{message.Site} Notify"),
                Subject = message.Subject,
                HtmlContent = GetHtmlContent(message)
            };

            foreach (var messageAttachment in message.Attachments)
            {
                await using (messageAttachment.Value)
                {
                    await msg.AddAttachmentAsync(messageAttachment.Key, messageAttachment.Value);
                }
            }

            msg.AddTo(new EmailAddress(message.ToEmailAddress, message.ToName));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogError(await response.Body.ReadAsStringAsync());
                return false;
            }

            _logger.LogInformation($"Email notify sent, subject = {message.Subject}.");
            return true;
        }

        private string GetHtmlContent(NotifyMessage message)
        {
            using var process = Process.GetCurrentProcess();
            var info = new StringBuilder();
            info.AppendLine($"<p>Timestamp: {message.Timestamp.ToChinaDateAndTime()}</p>");
            info.AppendLine($"<p>Memory: {ByteSize.FromBytes(process.PrivateMemorySize64).ToString("#.## MB")}</p>");
            info.AppendLine($"<p>CPU time: {process.TotalProcessorTime.ToDisplayString()}</p>");

            var footer = $"<div style='margin-top:1rem;font-size:smaller;color:grey;line-height: 1.2;'>{info}</div>";
            return footer;
        }
    }
}