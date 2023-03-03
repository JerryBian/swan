using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public class BlogPostObject : FileObjectBase, ISingleObject
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("isTopping")]
        public bool IsTopping { get; set; }

        [JsonPropertyName("containsMath")]
        public bool ContainsMath { get; set; }

        [JsonPropertyName("publishTime")]
        public DateTime PublishTime { get; set; }

        [JsonPropertyName("mdContent")]
        public string MdContent { get; set; }

        [JsonPropertyName("accessCount")]
        public int AccessCount { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; init; } = new();

        [JsonPropertyName("series")]
        public string Series { get; set; }

        public override string GetFileName()
        {
            return string.IsNullOrEmpty(Id) ? throw new Exception("Id is invalid.") : $"{Id}{Constants.Misc.JsonFileExt}";
        }
    }
}
