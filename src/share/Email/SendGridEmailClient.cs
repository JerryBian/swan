using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Laobian.Share.Email
{
    public class SendGridEmailClient : IEmailClient
    {
        private readonly SendGridClient _client;

        public SendGridEmailClient()
        {
            _client = new SendGridClient(Global.Config.Common.SendGridApiKey);
        }

        public async Task<bool> SendAsync(EmailEntry entry)
        {
            if (!Global.Environment.IsProduction())
            {
                entry.Subject = $"[{Global.Environment.EnvironmentName}]: {entry.Subject}";
            }

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