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
    public class SendGridEmailNotify : IEmailNotify
    {
        private readonly ILogger<SendGridEmailNotify> _logger;

        public SendGridEmailNotify(ILogger<SendGridEmailNotify> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendAsync(NotifyMessage message)
        {
            var sendGridMessage = message as SendGridEmailMessage;
            if (sendGridMessage == null)
            {
                _logger.LogError("Invalid message type for SendGrid.");
                return false;
            }

            if (string.IsNullOrEmpty(sendGridMessage.SendGridApiKey))
            {
                _logger.LogError($"No SendGrid Api Key provided. ==> {JsonUtil.Serialize(message)}");
                return false;
            }

            var client = new SendGridClient(sendGridMessage.SendGridApiKey);
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
            info.AppendLine(message.Content);
            info.AppendLine($"<p>Timestamp: {message.Timestamp.ToChinaDateAndTime()}</p>");
            info.AppendLine($"<p>Memory: {ByteSize.FromBytes(process.PrivateMemorySize64).ToString("#.## MB")}</p>");
            info.AppendLine($"<p>CPU time: {process.TotalProcessorTime.ToHuman()}</p>");

            var footer = $"<div style='margin-top:1rem;font-size:smaller;color:grey;line-height: 0.9;'>{info}</div>";
            return footer;
        }
    }
}