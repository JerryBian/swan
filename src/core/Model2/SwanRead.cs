using System.Text.Json.Serialization;

namespace Swan.Core.Model2
{
    public class SwanRead : SwanObject<SwanRead>
    { 
        [JsonPropertyName("book_name")]
        public string BookName { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("author_country")]
        public string AuthorCountry { get; set; }

        [JsonPropertyName("translator")]
        public string Translator { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("grade")]
        public int Grade { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}
