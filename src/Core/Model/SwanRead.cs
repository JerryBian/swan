using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanRead
    {
        #region Raw

        [JsonPropertyName("bookName")]
        public string BookName { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("authorCountry")]
        public string AuthorCountry { get; set; }

        [JsonPropertyName("translator")]
        public string Translator { get; set; }

        [JsonPropertyName("grade")]
        public short Grade { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("posts")]
        public List<string> Posts { get; set; } = [];

        #endregion

        #region Extension

        [JsonIgnore]
        public List<SwanPost> BlogPosts { get; } = [];

        [JsonIgnore]
        public string HtmlMetadata { get; set; }

        [JsonIgnore]
        public string HtmlComment { get; set; }

        #endregion
    }
}
