using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Microsoft.Extensions.Hosting;

namespace Laobian.Share
{
    public static class Global
    {
        public static ConcurrentStack<BlogAlertEntry> InMemoryLogQueue = new ConcurrentStack<BlogAlertEntry>();

        public static string RuntimeVersion = RuntimeInformation.FrameworkDescription;

        public static AppConfig Config { get; set; }

        public static DateTime StartTime { get; set; }

        public static IHostEnvironment Environment { get; set; }

        public static string AppVersion { get; set; }

        public static string RunningInterval => (DateTime.Now - StartTime).Human();
    }
}