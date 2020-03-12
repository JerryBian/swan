using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Laobian.Share.Config;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;

namespace Laobian.Share
{
    public static class Global
    {
        public static ConcurrentQueue<LogEntry> WarningLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> ErrorLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> CriticalLogQueue = new ConcurrentQueue<LogEntry>();

        public static string RuntimeVersion = RuntimeInformation.FrameworkDescription;

        public static AppConfig Config { get; set; }

        public static DateTime StartTime { get; set; }

        public static IHostEnvironment Environment { get; set; }
    }
}