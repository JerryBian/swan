using System.Threading.Tasks;

namespace Laobian.Share.Email
{
    public interface IEmailClient
    {
        Task<bool> SendAsync(EmailEntry entry);
    }
}
