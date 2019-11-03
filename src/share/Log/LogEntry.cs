using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class LogEntry
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTimeOffset When { get; set; }

        public LogLevel Level { get; set; }
    }
}
