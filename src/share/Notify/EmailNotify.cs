using System;
using System.Net;
using System.Threading.Tasks;
using Laobian.Share.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Laobian.Share.Notify
{
    public class EmailNotify : IEmailNotify
    {
        private readonly CommonConfig _config;
        private readonly ILogger<EmailNotify> _logger;

        public EmailNotify(IOptions<CommonConfig> config, ILogger<EmailNotify> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task<bool> SendAsync(NotifyMessage message)
        {
            if (string.IsNullOrEmpty(_config.SendGridApiKey))
            {
                Console.WriteLine($"No Api Key provided. ==> {JsonHelper.Serialize(message)}");
                return false;
            }

            var client = new SendGridClient(_config.SendGridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress($"{message.Site.ToString().ToLowerInvariant()}@laobian.me", $"{message.Site} Notify"),
                Subject = message.Subject,
                HtmlContent = message.Content
            };

            foreach (var messageAttachment in message.Attachments)
            {
                await using (messageAttachment.Value)
                {
                    await msg.AddAttachmentAsync(messageAttachment.Key, messageAttachment.Value);
                }
            }

            msg.AddTo(new EmailAddress(_config.AdminEmail, _config.AdminName));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogError(await response.Body.ReadAsStringAsync());
                return false;
            }

            _logger.LogInformation($"Email notify sent, subject = {message.Subject}.");
            return true;
        }
    }
}
