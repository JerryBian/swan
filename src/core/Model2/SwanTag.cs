using System.Text.Json.Serialization;

namespace Swan.Core.Model2
{
    public class SwanTag : SwanObject<SwanTag>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; }
    }
}
