using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Laobian.Share.Converter;
using Laobian.Share.Extension;

namespace Laobian.Share.Model.Jarvis;

[DataContract]
public class Diary
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime Date { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyOrder(4)]
    [JsonPropertyName("markdownContent")]
    public string MarkdownContent { get; set; }

    public string GetFullPath(string baseAddress)
    {
        return $"{baseAddress ?? string.Empty}/diary/{Date.Year:D4}/{Date.Month:D2}/{Date.Day:D2}.html";
    }

    public string GetDateString()
    {
        return Date.ToDate();
    }
}