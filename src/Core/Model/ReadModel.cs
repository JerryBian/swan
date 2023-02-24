using Swan.Core.Model.Object;
using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class ReadModel
    {
        public ReadModel() { }

        public ReadModel(ReadObject obj)
        {
            Object = obj;
        }

        [JsonPropertyName("o")]
        public ReadObject Object { get; init; }

        [JsonPropertyName("m")]
        public string Metadata { get; set; }

        [JsonPropertyName("h")]
        public string CommentHtml { get; set; }

        [JsonPropertyName("p")]
        public List<BlogPost> BlogPosts { get; init; } = new();
    }
}
