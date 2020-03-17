using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Config;
using Microsoft.Extensions.Hosting;

namespace Laobian.Share
{
    public static class Global
    {
        public static ConcurrentQueue<BlogAlertEntry> WarningLogQueue = new ConcurrentQueue<BlogAlertEntry>();

        public static ConcurrentQueue<BlogAlertEntry> ErrorLogQueue = new ConcurrentQueue<BlogAlertEntry>();

        public static string RuntimeVersion = RuntimeInformation.FrameworkDescription;

        public static AppConfig Config { get; set; }

        public static DateTime StartTime { get; set; }

        public static IHostEnvironment Environment { get; set; }
    }
}