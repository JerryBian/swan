using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share.Model.Jarvis;

[DataContract]
public class NoteTag
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyOrder(3)]
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyOrder(4)]
    [JsonPropertyName("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; set; }

    [DataMember(Order = 5)]
    [JsonPropertyOrder(5)]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}