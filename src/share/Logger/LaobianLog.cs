using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger;

public class LaobianLog
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("t")]
    public DateTime TimeStamp { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("n")]
    public string LoggerName { get; set; }

    [JsonPropertyOrder(3)]
    [JsonPropertyName("l")]
    public LogLevel Level { get; set; }

    [JsonPropertyOrder(4)]
    [JsonPropertyName("m")]
    public string Message { get; set; }

    [JsonPropertyOrder(5)]
    [JsonPropertyName("e")]
    public string Exception { get; set; }

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