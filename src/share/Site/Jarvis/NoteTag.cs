using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Jarvis;

public class NoteTag
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyOrder(2)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyOrder(3)]
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyOrder(4)]
    [JsonPropertyName("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; set; }

    [JsonPropertyOrder(5)]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}