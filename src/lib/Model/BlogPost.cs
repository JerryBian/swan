using System.Text.Json.Serialization;

namespace Swan.Lib.Model
{
    public class BlogPost
    {
        [JsonPropertyOrder(0)]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyOrder(1)]
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyOrder(2)]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyOrder(3)]
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        [JsonPropertyOrder(4)]
        [JsonPropertyName("isTopping")]
        public bool IsTopping { get; set; }

        [JsonPropertyOrder(5)]
        [JsonPropertyName("containsMath")]
        public bool ContainsMath { get; set; }

        [JsonPropertyOrder(6)]
        [JsonPropertyName("createTime")]
        public DateTime CreateTime { get; set; }

        [JsonPropertyOrder(7)]
        [JsonPropertyName("publishTime")]
        public DateTime PublishTime { get; set; }

        [JsonPropertyOrder(8)]
        [JsonPropertyName("lastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }

        [JsonPropertyOrder(9)]
        [JsonPropertyName("mdContent")]
        public string MdContent { get; set; }

        [JsonPropertyOrder(10)]
        [JsonPropertyName("accessCount")]
        public int AccessCount { get; set; }
    }
}
