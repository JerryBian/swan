using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Logger;

[DataContract]
public class LaobianLog
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("t")]
    public DateTime TimeStamp { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("n")]
    public string LoggerName { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("l")]
    public LogLevel Level { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyOrder(4)]
    [JsonPropertyName("m")]
    public string Message { get; set; }

    [DataMember(Order = 5)]
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