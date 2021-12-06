using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Jarvis;

public class Note
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyOrder(3)]
    [JsonPropertyName("tag")]
    public List<string> Tag { get; set; } = new();

    [JsonPropertyOrder(4)]
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [JsonPropertyOrder(5)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [JsonPropertyOrder(6)]
    [JsonPropertyName("mdContent")]
    public string MdContent { get; set; }

    public string GetFullPath()
    {
        var path =
            $"/note/{Link}.html";
        return path;
    }
}