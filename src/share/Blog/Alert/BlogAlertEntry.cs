using Laobian.Share.Log;

namespace Laobian.Share.Blog.Alert
{
    public class BlogAlertEntry : LogEntry
    {
        public string Ip { get; set; }

        public string RequestUrl { get; set; }

        public string UserAgent { get; set; }
    }
}