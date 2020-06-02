using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class LogEntry
    {
        public DateTime When { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public LogLevel Level { get; set; }
    }
}
