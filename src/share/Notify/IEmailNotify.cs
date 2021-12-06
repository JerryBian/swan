using System.Threading.Tasks;

namespace Laobian.Share.Notify;

public interface IEmailNotify
{
    Task<bool> SendAsync(NotifyMessage message);
}