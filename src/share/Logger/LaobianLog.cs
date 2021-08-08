using System;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger
{
    public class LaobianLog
    {
        public string Message { get; set; }

        public string Exception { get; set; }

        public DateTime TimeStamp { get; set; }

        public string UserAgent { get; set; }

        public string RequestIp { get; set; }

        public string RequestUrl { get; set; }

        public LogLevel Level { get; set; }

        public string LoggerName { get; set; }

        public void Clone(LaobianLog log)
        {
            foreach (var propertyInfo in typeof(LaobianLog).GetProperties())
            {
                var defaultValue = propertyInfo.PropertyType.IsValueType
                    ? Activator.CreateInstance(propertyInfo.PropertyType)
                    : null;
                var logValue = propertyInfo.GetValue(log);
                if (defaultValue != logValue) propertyInfo.SetValue(this, logValue);
            }
        }
    }
}