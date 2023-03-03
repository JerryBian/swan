using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public class ReadObject : FileObjectBase
    {
        [JsonPropertyName("bookName")]
        public string BookName { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("authorCountry")]
        public string AuthorCountry { get; set; }

        [JsonPropertyName("translator")]
        public string Translator { get; set; }

        [JsonPropertyName("publisherName")]
        public string PublisherName { get; set; }

        [JsonPropertyName("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonPropertyName("grade")]
        public int Grade { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("posts")]
        public List<string> Posts { get; init; } = new();

        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        public override string GetFileName()
        {
            return CreateTime == default ? throw new Exception("Create time is invalid.") : $"{CreateTime:yyyy}{Constants.Misc.JsonFileExt}";
        }
    }
}
