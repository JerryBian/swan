using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class SwanRead : SwanObject
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
        public List<string> Posts { get; set; } = new();

        #endregion

        #region Extension

        [JsonIgnore]
        public List<SwanPost> BlogPosts { get; } = new();

        [JsonIgnore]
        public string HtmlMetadata { get; set; }

        [JsonIgnore]
        public string HtmlComment { get; set; }

        #endregion

        public override string GetGitStorePath() => "obj/read.json";

        public override string GetFullLink() => $"/read#link-{Id}";
    }
}
