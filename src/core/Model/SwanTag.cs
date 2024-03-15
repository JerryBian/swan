using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanTag : ISwanObject
    {
        #region Object

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("last_modified_at")]
        public DateTime LastModifiedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

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

        #endregion

        #region ViewModels

        [JsonIgnore]
        public List<SwanPost> Posts { get; init; } = [];

        #endregion

        public static string GitPath => "data/tag.json";
    }
}
