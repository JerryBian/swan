using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Log;

namespace Laobian.Share.Blog.Alert
{
    public interface IBlogAlertService
    {
        Task AlertWarningsAsync(string subject, List<LogEntry> entries);

        Task AlertCriticalAsync(string subject, List<LogEntry> entries);

        Task AlertErrorsAsync(string subject, List<LogEntry> entries);

        Task AlertEventAsync(string message, Exception error = null);

        Task AlertReportAsync(string message, Dictionary<string, Stream> logs);
    }
}