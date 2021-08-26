using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Site.Blog
{
    public class BlogTag
    {
        [JsonPropertyName("displayName")] public string DisplayName { get; set; }

        [JsonPropertyName("link")] public string Link { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("lastUpdatedAt")] public DateTime LastUpdatedAt { get; set; }
    }
}