using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog;

[DataContract]
public class BlogTag
{
    [DataMember(Order = 1)]
    [JsonPropertyName("id")]
    [JsonPropertyOrder(1)]
    public string Id { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyName("displayName")]
    [JsonPropertyOrder(2)]
    public string DisplayName { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyName("link")]
    [JsonPropertyOrder(3)]
    public string Link { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyName("description")]
    [JsonPropertyOrder(4)]
    public string Description { get; set; }

    [DataMember(Order = 5)]
    [JsonPropertyName("lastUpdatedAt")]
    [JsonPropertyOrder(5)]
    public DateTime LastUpdatedAt { get; set; }
}