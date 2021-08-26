using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger
{
    public class LaobianLog
    {
        [JsonPropertyName("m")] public string Message { get; set; }

        [JsonPropertyName("e")] public string Exception { get; set; }

        [JsonPropertyName("t")] public DateTime TimeStamp { get; set; }

        [JsonPropertyName("l")] public LogLevel Level { get; set; }

        [JsonPropertyName("n")] public string LoggerName { get; set; }

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