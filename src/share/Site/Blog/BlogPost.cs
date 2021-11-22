using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog;

public class BlogPost
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyOrder(3)]
    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }

    [JsonPropertyOrder(4)]
    [JsonPropertyName("isTopping")]
    public bool IsTopping { get; set; }

    [JsonPropertyOrder(5)]
    [JsonPropertyName("containsMath")]
    public bool ContainsMath { get; set; }

    [JsonPropertyOrder(6)]
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }

    [JsonPropertyOrder(7)]
    [JsonPropertyName("publishTime")]
    public DateTime PublishTime { get; set; }

    [JsonPropertyOrder(8)]
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }

    [JsonPropertyOrder(9)]
    [JsonPropertyName("tag")]
    public List<string> Tag { get; set; } = new();

    [JsonPropertyOrder(10)]
    [JsonPropertyName("excerpt")]
    public string Excerpt { get; set; }

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
            $"{baseAddress}/{Link}.html";
        return path;
    }
}