using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share.Model.Blog;

[DataContract]
public class BlogPost
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyOrder(4)]
    [JsonPropertyName("isTopping")]
    public bool IsTopping { get; set; }

    [DataMember(Order = 5)]
    [JsonPropertyOrder(5)]
    [JsonPropertyName("containsMath")]
    public bool ContainsMath { get; set; }

    [DataMember(Order = 6)]
    [JsonPropertyOrder(6)]
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [DataMember(Order = 7)]
    [JsonPropertyOrder(7)]
    [JsonPropertyName("publishTime")]
    public DateTime PublishTime { get; set; }

    [DataMember(Order = 8)]
    [JsonPropertyOrder(8)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [DataMember(Order = 9)]
    [JsonPropertyOrder(9)]
    [JsonPropertyName("tag")]
    public List<string> Tag { get; set; } = new();

    [DataMember(Order = 10)]
    [JsonPropertyOrder(10)]
    [JsonPropertyName("excerpt")]
    public string Excerpt { get; set; }

    [DataMember(Order = 11)]
    [JsonPropertyOrder(11)]
    [JsonPropertyName("mdContent")]
    public string MdContent { get; set; }

    public bool IsPostPublished()
    {
        return IsPublished && PublishTime <= DateTime.Now;
    }

    public string GetFullPath(string baseAddress)
    {
        baseAddress = string.IsNullOrEmpty(baseAddress) ? string.Empty : baseAddress;
        var path =
            $"{baseAddress}/{PublishTime.Year:D4}/{PublishTime.Month:D2}/{Link}.html";
        return path;
    }
}