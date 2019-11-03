using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Log
{
    public interface ILogService
    {
        Task LogInformation(string message, bool alert = false);

        Task LogWarning(string message, Exception ex = null, bool alert = false);

        Task LogError(string message, Exception ex = null, bool alert = false);
    }
}
