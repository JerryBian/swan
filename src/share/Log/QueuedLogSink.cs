//using System;
//using Laobian.Share.Blog.Alert;
//using Microsoft.Extensions.Logging;
//using Serilog.Core;
//using Serilog.Events;

//namespace Laobian.Share.Log
//{
//    public class QueuedLogSink : ILogEventSink
//    {
//        private readonly IFormatProvider _formatProvider;

//        public QueuedLogSink(IFormatProvider formatProvider)
//        {
//            _formatProvider = formatProvider;
//        }

//        public void Emit(LogEvent logEvent)
//        {
//            if (logEvent.Level < LogEventLevel.Warning)
//            {
//                return;
//            }

//            var entry = new BlogAlertEntry
//            {
//                Exception = logEvent.Exception,
//                Message = logEvent.RenderMessage(_formatProvider),
//                When = DateTime.Now,
//                Level = logEvent.Level == LogEventLevel.Warning ? LogLevel.Warning : LogLevel.Error
//        };

//            if (logEvent.Properties.ContainsKey(LogEntryItem.RequestUrl.ToString()))
//            {
//                entry.RequestUrl = logEvent.Properties[LogEntryItem.RequestUrl.ToString()].ToString();
//            }

//            if (logEvent.Properties.ContainsKey(LogEntryItem.RequestIp.ToString()))
//            {
//                entry.Ip = logEvent.Properties[LogEntryItem.RequestIp.ToString()].ToString();
//            }

//            if (logEvent.Properties.ContainsKey(LogEntryItem.RequestUserAgent.ToString()))
//            {
//                entry.UserAgent = logEvent.Properties[LogEntryItem.RequestUserAgent.ToString()].ToString();
//            }

//            Global.InMemoryLogQueue.Push(entry);
//        }
//    }
//}

