using System.Threading.Tasks;

namespace Laobian.Share.Infrastructure.Email
{
    public interface IEmailClient
    {
        Task<bool> SendAsync(
            string from,
            string fromAddress,
            string to,
            string toAddress,
            string subject,
            string htmlContent);

        Task<bool> SendAsync(EmailEntry entry);
    }
}
