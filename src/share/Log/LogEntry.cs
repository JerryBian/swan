using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class LogEntry
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTime When { get; set; }

        public LogLevel Level { get; set; }
    }
}
