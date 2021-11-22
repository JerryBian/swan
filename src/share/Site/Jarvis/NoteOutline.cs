using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Jarvis;

public class NoteOutline
{
    [JsonPropertyName("link")] public string Link { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }
}