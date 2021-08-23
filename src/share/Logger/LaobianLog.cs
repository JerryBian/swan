using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger
{
    public class LaobianLog
    {
        [JsonPropertyName("message")] public string Message { get; set; }

        [JsonPropertyName("exception")] public string Exception { get; set; }

        [JsonPropertyName("timestamp")] public DateTime TimeStamp { get; set; }

        [JsonPropertyName("level")] public LogLevel Level { get; set; }

        [JsonPropertyName("logger")] public string LoggerName { get; set; }

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