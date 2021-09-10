﻿using System.Text.Json.Serialization;

namespace Laobian.Share.HtmlHead
{
    public class GoogleStructuredAuthor
    {
        [JsonPropertyName("@type")] public string Type { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("url")] public string Url { get; set; }
    }
}