using System;
using System.Collections.Concurrent;
using System.Reflection;
using Laobian.Share.Extension;
using Laobian.Share.Log;

namespace Laobian.Share
{
    public static class MemoryStore
    {
        public static ConcurrentQueue<LogEntry> WarningLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> ErrorLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> CriticalLogQueue = new ConcurrentQueue<LogEntry>();

        public static DateTime StartTime;

        public static string RuntimeString => (DateTime.Now - StartTime).Human();

        public static TimeSpan Runtime => DateTime.Now - StartTime;

        public static string Version = Assembly.GetEntryAssembly()?.GetName().Version.ToString();

        public static string NetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    }
}
