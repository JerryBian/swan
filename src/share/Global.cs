﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using Laobian.Share.Config;
using Laobian.Share.Extension;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;

namespace Laobian.Share
{
    public static class Global
    {
        private static string _version;

        public static ConcurrentQueue<LogEntry> WarningLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> ErrorLogQueue = new ConcurrentQueue<LogEntry>();

        public static ConcurrentQueue<LogEntry> CriticalLogQueue = new ConcurrentQueue<LogEntry>();

        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_version))
                {
                    _version = !string.IsNullOrEmpty(Config.Common.Version)
                        ? Config.Common.Version
                        : Assembly.GetEntryAssembly()?.GetName().Version.ToString();
                }

                return _version;
            }
        }

        public static AppConfig Config { get; set; }

        public static DateTime StartTime { get; set; }

        public static string RuntimeString => (DateTime.Now - StartTime).Human();

        public static TimeSpan Runtime => DateTime.Now - StartTime;

        public static IHostEnvironment Environment { get; set; }
    }
}