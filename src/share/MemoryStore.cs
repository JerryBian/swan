using System;
using System.Collections.Concurrent;
using System.Reflection;
using Laobian.Share.Log;

namespace Laobian.Share
{
    public static class MemoryStore
    {
        public static ConcurrentQueue<LogEntry> LogQueue = new ConcurrentQueue<LogEntry>();

        public static DateTime StartTime;

        public static string RunTime => (DateTime.Now - StartTime).ToString();

        public static string Version = Assembly.GetEntryAssembly()?.GetName().Version.ToString();

        public static string NetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    }
}
