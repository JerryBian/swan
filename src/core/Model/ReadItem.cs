using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class ReadItem : SwanObject
    {
        public const string GitStorePath = "obj/read.json";

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
        public List<string> Posts { get; init; } = new();

        #endregion

        #region Extension

        [JsonIgnore]
        public List<BlogPost> BlogPosts { get; } = new();

        #endregion

        public string GetFullLink() => $"/read/{CreatedAt.Year}/{Id}";
    }
}
