using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Laobian.Lib.Converter;

namespace Laobian.Lib.HtmlHead;

public class GoogleStructuredData
{
    [JsonPropertyName("@context")] public string Context { get; set; }

    [JsonPropertyName("@type")] public string Type { get; set; }

    [JsonPropertyName("headline")] public string Headline { get; set; }

    [JsonPropertyName("image")] public List<string> Images { get; } = new();

    [JsonPropertyName("datePublished")]
    [JsonConverter(typeof(IsoDateTimeZoneConverter))]
    public DateTime DatePublished { get; set; }

    [JsonPropertyName("dateModified")]
    [JsonConverter(typeof(IsoDateTimeZoneConverter))]
    public DateTime DateModified { get; set; }

    [JsonPropertyName("author")] public List<GoogleStructuredAuthor> Authors { get; } = new();
}