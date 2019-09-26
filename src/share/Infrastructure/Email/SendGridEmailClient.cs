using System.Net;
using System.Threading.Tasks;
using Laobian.Share.Config;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Laobian.Share.Infrastructure.Email
{
    public class SendGridEmailClient : IEmailClient
    {
        private readonly SendGridClient _client;

        public SendGridEmailClient(IOptions<AppConfig> appConfig)
        {
            _client = new SendGridClient(appConfig.Value.SendGridApiKey);
        }

        public async Task<bool> SendAsync(string @from, string fromAddress, string to, string toAddress, string subject, string htmlContent)
        {
            var fromEmailAddress = new EmailAddress(from, fromAddress);
            var toEmailAddress = new EmailAddress(to, toAddress);
            var message =
                MailHelper.CreateSingleEmail(fromEmailAddress, toEmailAddress, subject, htmlContent, htmlContent);
            var response = await _client.SendEmailAsync(message);
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
