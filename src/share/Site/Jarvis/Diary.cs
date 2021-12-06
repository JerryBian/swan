using System;
using System.Text.Json.Serialization;
using Laobian.Share.Converter;
using Laobian.Share.Option;

namespace Laobian.Share.Site.Jarvis;

public class Diary
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime Date { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [JsonPropertyOrder(3)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [JsonPropertyOrder(4)]
    [JsonPropertyName("markdownContent")]
    public string MarkdownContent { get; set; }

    public string GetFullPath(SharedOptions option)
    {
        return $"{option.JarvisRemoteEndpoint}/diary/{Date.Year:D4}/{GetDateString()}.html";
    }

    public string GetDateString()
    {
        return Date.ToString("yyyy-MM-dd");
    }
}