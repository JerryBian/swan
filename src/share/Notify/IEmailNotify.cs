using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Notify
{
    public interface IEmailNotify
    {
        Task<bool> SendAsync(NotifyMessage message);
    }
}
