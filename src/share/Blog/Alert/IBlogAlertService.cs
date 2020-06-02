using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Log;

namespace Laobian.Share.Blog.Alert
{
    public interface IBlogAlertService
    {
        Task AlertEventAsync(string message, Exception error = null);

        Task AlertLogsAsync(List<LogEntry> logs);
    }
}