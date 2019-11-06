using System.Net;
using System.Threading.Tasks;
using Laobian.Share.Config;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Laobian.Share.Email
{
    public class SendGridEmailClient : IEmailClient
    {
        private readonly SendGridClient _client;

        public SendGridEmailClient(IOptions<AppConfig> appConfig)
        {
            _client = new SendGridClient(appConfig.Value.Common.SendGridApiKey);
        }

        public async Task<bool> SendAsync(EmailEntry entry)
        {
            if (string.IsNullOrEmpty(entry.PlainContent) && !string.IsNullOrEmpty(entry.HtmlContent))
            {
                entry.PlainContent = entry.HtmlContent;
            }

            if (string.IsNullOrEmpty(entry.HtmlContent) && !string.IsNullOrEmpty(entry.PlainContent))
            {
                entry.HtmlContent = entry.PlainContent;
            }

            var from = new EmailAddress(entry.FromAddress, entry.FromName);
            var to = new EmailAddress(entry.ToAddress, entry.ToName);
            var message =
                MailHelper.CreateSingleEmail(from, to, entry.Subject, entry.PlainContent, entry.HtmlContent);
            var response = await _client.SendEmailAsync(message);
            return response.StatusCode == HttpStatusCode.Accepted;
        }
    }
}
