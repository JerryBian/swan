using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.HtmlHead
{
    public class GoogleStructuredData
    {
        [JsonPropertyName("@context")] public string Context { get; set; }

        [JsonPropertyName("@type")] public string Type { get; set; }

        [JsonPropertyName("headline")] public string Headline { get; set; }

        [JsonPropertyName("image")] public List<string> Images { get; } = new();

        [JsonPropertyName("datePublished")] public DateTime DatePublished { get; set; }

        [JsonPropertyName("dateModified")] public DateTime DateModified { get; set; }

        [JsonPropertyName("author")] public List<GoogleStructuredAuthor> Authors { get; } = new();
    }
}