using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Jarvis;

[DataContract]
public class NoteOutline
{
    [DataMember(Order = 1)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyOrder(2)]
    [JsonPropertyName("title")]
    public string Title { get; set; }
}