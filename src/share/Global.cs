using System;
using System.Collections.Concurrent;
<<<<<<< HEAD
=======
using System.Reflection;
>>>>>>> master
using System.Runtime.InteropServices;
using Laobian.Share.Config;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;

namespace Laobian.Share
{
    public static class Global
    {
<<<<<<< HEAD
=======
        private static string _version;
        private static readonly DateTime BlogFirstStartTime = new DateTime(2012, 07, 01);

>>>>>>> master
        public static ConcurrentQueue<LogEntry> WarningLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> ErrorLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> CriticalLogQueue = new ConcurrentQueue<LogEntry>();

        public static AppConfig Config { get; set; }

        public static DateTime StartTime { get; set; }

<<<<<<< HEAD
=======
        public static string RuntimeString => (DateTime.Now - BlogFirstStartTime).Human();

        public static TimeSpan Runtime => DateTime.Now - StartTime;

>>>>>>> master
        public static IHostEnvironment Environment { get; set; }

        public static string RuntimeVersion = RuntimeInformation.FrameworkDescription;
    }
}