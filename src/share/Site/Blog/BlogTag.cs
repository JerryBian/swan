using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog;

public class BlogTag
{
    [JsonPropertyName("id")]
    [JsonPropertyOrder(1)]
    public string Id { get; set; }

    [JsonPropertyName("displayName")]
    [JsonPropertyOrder(2)]
    public string DisplayName { get; set; }

    [JsonPropertyName("link")]
    [JsonPropertyOrder(3)]
    public string Link { get; set; }

    [JsonPropertyName("description")]
    [JsonPropertyOrder(4)]
    public string Description { get; set; }

    [JsonPropertyName("lastUpdatedAt")]
    [JsonPropertyOrder(5)]
    public DateTime LastUpdatedAt { get; set; }
}