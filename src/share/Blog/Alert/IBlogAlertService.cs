using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laobian.Share.Blog.Alert
{
    public interface IBlogAlertService
    {
        Task AlertEventAsync(string message, Exception error = null);

        Task AlertReportAsync(List<BlogAlertEntry> warnAlerts, List<BlogAlertEntry> errorAlerts);
    }
}