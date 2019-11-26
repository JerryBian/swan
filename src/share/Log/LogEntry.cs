using System;

namespace Laobian.Share.Log
{
    public class LogEntry
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTime When { get; set; }
    }
}