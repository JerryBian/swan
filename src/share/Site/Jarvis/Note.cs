using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Laobian.Share.Option;

namespace Laobian.Share.Site.Jarvis;

[DataContract]
public class Note
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("tag")]
    public List<string> Tags { get; set; } = new();

    [DataMember(Order = 4)]
    [JsonPropertyOrder(4)]
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [DataMember(Order = 5)]
    [JsonPropertyOrder(5)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [DataMember(Order = 6)]
    [JsonPropertyOrder(6)]
    [JsonPropertyName("mdContent")]
    public string MdContent { get; set; }

    public string GetFullPath(SharedOptions options)
    {
        var path =
            $"{options.JarvisRemoteEndpoint}/note/{Id}.html";
        return path;
    }
}