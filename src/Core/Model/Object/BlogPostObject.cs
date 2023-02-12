using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public class BlogPostObject : FileObjectBase
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

        public override string GetFileName()
        {
            if(string.IsNullOrEmpty(Id))
            {
                throw new Exception("Is is invalid.");
            }

            return $"{Id}{Constants.JsonFileExt}";
        }

        public string GetUrl()
        {
            return $"/blog/{Link}.html";
        }

        public bool IsPublished()
        {
            return PublishTime <= DateTime.Now;
        }
    }
}
