using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class BlogSeries : SwanObject
    {
        public const string GitStorePath = "obj/blog/series.json";

        #region Raw

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("description")]
        public string Descprition { get; set; }

        #endregion

        #region Extension

        [JsonIgnore]
        public List<BlogPost> BlogPosts { get; } = new();

        #endregion
    }
}
