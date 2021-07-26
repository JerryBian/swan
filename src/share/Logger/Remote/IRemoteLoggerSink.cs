using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laobian.Share.Logger.Remote
{
    public interface IRemoteLoggerSink
    {
        Task SendAsync(string loggerName, IEnumerable<LaobianLog> logs);
    }
}